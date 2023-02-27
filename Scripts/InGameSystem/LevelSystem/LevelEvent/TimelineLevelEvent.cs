using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
namespace Olympus
{
    public class TimelineLevelEvent : LevelEventBase
    {
        public PlayableDirector timeline;
        public LevelEventBase endLevelEvent;

        private Coroutine skipTimelineRoutine = null;

        public bool isHideUI = true;
        public bool isAutoStop = true;
        public bool isPossibleTimelineCancle = true;

        public float skipTime = 2.0f;
        public float skipTimer = 0.0f;

        public override void OnLevelEvent(EntityBase entity)
        {
            if (isHideUI)
            {
                UIManager.SaveCurrentStates();
            }

            PlayerController.Instance.InputLock(LockType.FromTimeline);
            if (PlayerController.Instance.PlayerEntity)
            {
                PlayerController.Instance.PlayerEntity.EntityData.godModeTimer = 9999999;
            }

            if (isAutoStop)
            {
                timeline.stopped -= TimelineEnd;
                timeline.stopped += TimelineEnd;
            }

            TimelineAsset timelineAsset = timeline.playableAsset as TimelineAsset;

            int trackCount = timelineAsset.outputTrackCount;

            for (int i = 0; i < trackCount; i++)
            {
                FMODUnity.FMODEventTrack fmodTrack = timelineAsset.GetOutputTrack(i) as FMODUnity.FMODEventTrack;

                if (fmodTrack != null)
                {
                    float sfxVolume = SoundManager.Instance.AllSoundsVolume[SoundType.SFX];
                    float masterVolume = SoundManager.Instance.AllSoundsVolume[SoundType.All];
                    fmodTrack.template.volume = sfxVolume * masterVolume;
                }
            }

            timeline.gameObject.SetActive(true);


            if (isPossibleTimelineCancle)
            {
                if (skipTimelineRoutine != null)
                {
                    StopCoroutine(skipTimelineRoutine);
                    skipTimelineRoutine = null;
                }

                TimelineSkipUI skipUI = UIManager.Show(UIList.TimelineSkipUI) as TimelineSkipUI;
                skipTimelineRoutine = StartCoroutine(skipTimeline(skipUI));

                IEnumerator skipTimeline(TimelineSkipUI skipUI)
                {
                    skipTimer = 0.0f;
                    while (timeline.state == PlayState.Playing)
                    {
                        if (skipUI)
                        {
                            if (Input.GetKey(KeyCode.Escape))
                            {
                                skipTimer += Time.deltaTime;
                                if (skipTimer >= skipTime)
                                {
                                    timeline.Stop();
                                }
                            }
                            else
                            {
                                skipTimer -= Time.deltaTime;
                                if (skipTimer < 0.0f)
                                {
                                    skipTimer = 0.0f;
                                }
                            }

                            skipUI.SetSkipFill(skipTimer / skipTime);
                        }
                        yield return null;
                    }
                }
            }
        }

        public void TimelineEnd(PlayableDirector pd)
        {
            if (isHideUI)
            {
                UIManager.RecoverPreviousStatus();
            }
            endLevelEvent?.OnLevelEvent(null);
            timeline.gameObject.SetActive(false);

            PlayerController.Instance.InputUnLock(LockType.FromTimeline);
            if (PlayerController.Instance.PlayerEntity)
            {
                PlayerController.Instance.PlayerEntity.EntityData.godModeTimer = 0;
            }

            if (skipTimelineRoutine != null)
            {
                StopCoroutine(skipTimelineRoutine);
                skipTimelineRoutine = null;
            }

            UIManager.Hide(UIList.TimelineSkipUI);
        }
    }
}