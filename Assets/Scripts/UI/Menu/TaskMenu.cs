using RecoDeli.Scripts.Assets.Scripts.Leaderboards;
using RecoDeli.Scripts.Leaderboards;
using RecoDeli.Scripts.Level;
using RecoDeli.Scripts.SaveManagement;
using RecoDeli.Scripts.Settings;
using RecoDeli.Scripts.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace RecoDeli.Scripts.UI.Menu
{
    public class TaskMenu : ModalWindow
    {
        [SerializeField] private StatsDisplayer statsDisplayer;
        [SerializeField] private MainMenuInterface mainMenu;

        private ScrollView taskList;
        private VisualElement taskContainer;
        private VisualElement taskInfoFrom;
        private VisualElement taskInfoTo;
        private VisualElement taskInfoTitle;
        private ScrollView taskDescription;
        private Button startTaskButton;

        private static bool hasBeenOpenedBefore = false;
        private static GameTask currentTask;

        private Dictionary<string, LeaderboardProvider> providerCache = new();
        private Dictionary<GameTask, Button> taskButtonCache = new();

        

        protected override void Awake()
        {
            base.Awake();

            taskList = RootElement.Q<ScrollView>("tasks-list");
            taskContainer = RootElement.Q<VisualElement>("task-details");
            taskInfoFrom = RootElement.Q<VisualElement>("task-info-from");
            taskInfoTo = RootElement.Q<VisualElement>("task-info-to");
            taskInfoTitle = RootElement.Q<VisualElement>("task-info-title");
            taskDescription = RootElement.Q<ScrollView>("task-description");
            startTaskButton = RootElement.Q<Button>("start-task-button");

            startTaskButton.clicked += LoadIntoCurrentTask;

            statsDisplayer.Initialize(windowDocument);

            CreateTaskList();

            //Open();
        }

        public override void Open()
        {
            base.Open();
            providerCache.Clear();

            CreateTaskList();

            if (!hasBeenOpenedBefore)
            {
                OpenTask(GameTaskDictionary.Instance.Collections[0].Tasks[0]);
            }
            else
            {
                OpenTask(currentTask);
            }

            hasBeenOpenedBefore = true;
        }

        private void CreateTaskList()
        {
            taskList.Clear();
            taskButtonCache.Clear();

            var completedLevels = SaveManager.CurrentSave.CompletedLevelsCount;

            foreach (var taskCollection in GameTaskDictionary.Instance.Collections)
            {
                var collectionFoldout = new Foldout();
                collectionFoldout.text = taskCollection.Name;
                taskList.Add(collectionFoldout);

                foreach(var task in taskCollection.Tasks)
                {
                    if (task.RequiredCompletedLevels > completedLevels) continue;

                    var taskButton = new Button();
                    taskButton.text = task.Title;

                    if(task.Action == GameTask.ActionType.Task)
                    {
                        var levelName = task.ActionParameter;
                        if (SaveManager.CurrentSave.IsLevelComplete(levelName))
                        {
                            taskButton.AddToClassList("task-button-complete");

                            if (SaveManager.CurrentSave.AreAllLevelsCompleted())
                            {
                                var levelInfo = SaveManager.CurrentSave.GetLevelInfo(levelName);
                                var authorTime = AuthorRecords.Instance.Records.Where(r => r.MapName == levelName).First();

                                if (levelInfo.FastestTime <= authorTime.Time && levelInfo.LowestInstructions <= authorTime.Instructions)
                                {
                                    taskButton.AddToClassList("task-button-golden");
                                }
                            }
                        }
                    }


                    taskButton.clicked += () => OpenTask(task);
                    collectionFoldout.Add(taskButton);

                    taskButtonCache[task] = taskButton;
                }
            }
        }

        private void LockButton(Button button)
        {
            foreach(var taskListChild in taskList.Children())
            {
                if (taskListChild is not Foldout taskFoldout) continue;

                foreach(var taskFoldoutChild in taskFoldout.Children())
                {
                    taskFoldoutChild.SetEnabled(taskFoldoutChild != button);
                }
            }
        }

        private void OpenTask(GameTask task)
        {
            if (taskButtonCache.ContainsKey(task))
            {
                LockButton(taskButtonCache[task]);
            }

            currentTask = task;

            // clear all info
            taskInfoFrom.Clear();
            taskInfoTo.Clear();
            taskInfoTitle.Clear();
            taskDescription.Clear();

            var userName = SaveManager.CurrentSave.Name;

            taskInfoFrom.Add(new Label(task.SenderName));
            var senderAddress = new Label(task.SenderAddress);
            senderAddress.AddToClassList("task-info-mail");
            taskInfoFrom.Add(senderAddress);

            taskInfoTo.Add(new Label("You"));
            var receiverAddress = new Label(userName.ToLower() + "@recodeli.com");
            receiverAddress.AddToClassList("task-info-mail");
            taskInfoTo.Add(receiverAddress);

            if(task.AdditionalReceivers > 0)
            {
                taskInfoFrom.Add(new Label($"and {task.AdditionalReceivers} others"));
            }

            taskInfoTitle.Add(new Label(task.Title));

            taskDescription.Add(new Label(task.Description));

            taskContainer.EnableInClassList("taskless", task.Action != GameTask.ActionType.Task);
            taskContainer.EnableInClassList("actionless", task.Action == GameTask.ActionType.None);

            if(task.Action != GameTask.ActionType.None)
            {
                var levelName = currentTask.ActionParameter;
                if (!providerCache.ContainsKey(levelName))
                {
                    var provider = RecoDeliGame.CreateNewLeaderboardProvider(levelName);
                    providerCache[levelName] = provider;
                    provider.RequestScores();
                }
                statsDisplayer.SetProvider(providerCache[levelName]);

                var levelInfo = SaveManager.CurrentSave.GetLevelInfo(levelName);
                if(levelInfo == null)
                {
                    statsDisplayer.SetStats(false, 0,0,0);
                }
                else
                {
                    statsDisplayer.SetStats(levelInfo.Completed, levelInfo.FastestTime, levelInfo.LowestInstructions, levelInfo.ExecutionCount);
                }
            }
        }

        private void LoadIntoCurrentTask()
        {
            if (currentTask.Action == GameTask.ActionType.Task)
            {
                var levelName = currentTask.ActionParameter;
                var loadingText = $"Loading \"{currentTask.ActionParameter}\"...";
                mainMenu.StartLoadingScreen(loadingText, () =>
                {
                    RecoDeliGame.OpenLevel(levelName);
                });
            }
        }
    }
}
