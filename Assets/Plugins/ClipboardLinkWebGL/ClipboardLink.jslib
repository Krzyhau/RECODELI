// a quick hack to allow copy-pasting in webgl builds.

var ClipboardString = "";
var ClipboardInaccessible = false;

const loadClipboardRoutine = setInterval(() => {
    navigator.clipboard.readText()
        .then(text => {
            ClipboardString = text;
        })
        .catch(err => {
            console.error('Failed to read clipboard contents: ', err);
            clearInterval(loadClipboardRoutine);
            ClipboardInaccessible = true;
        });
}, 500);

mergeInto(LibraryManager.library, {
    GetClipboardContent: function () {
        return ClipboardString;
    },
    SetClipboardContent: function (clipboardContent) {
        ClipboardString = clipboardContent;
        navigator.clipboard.writeText(clipboardContent);
    },
    IsClipboardAccessible: function () {
        return !ClipboardInaccessible;
    }
});