using UnityEngine;

namespace RecoDeli.Scripts.Controllers
{
    public class AudioManager : MonoBehaviour
    {

        public AudioClip newClip;

        AudioClip oldClip;
        AudioSource audios;

        void Start()
        {
            audios = GetComponent<AudioSource>();
        }

        void LateUpdate()
        {
            if (newClip == null)
            {
                oldClip = null;
            }
            if (newClip != null && newClip != oldClip)
            {
                audios.PlayOneShot(newClip);
                oldClip = newClip;
                newClip = null;

            }
        }

        public void PlayClip(AudioClip clip)
        {
            if (!clip) return;
            audios.PlayOneShot(clip);
        }
    }
}

