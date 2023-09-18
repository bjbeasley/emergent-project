using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(RawImage))]
public class TurnIcon : MonoBehaviour
{
    private Agent agent;

    private RawImage image;
    private RectTransform rectTransform;

    private bool fadeInProgress = false;
    private float fadeSpeed = 1;
    private bool shiftInProgress = false;
    private float shiftSpeed = 1;
    private bool scaleInProgress = false;
    private float scaleSpeed = 1;

    private int lastIndex = -1;

    private TurnInterface turnInterface;

    private float opacity = 0;

    [SerializeField]
    private PortraitToken token;

    private bool tokenFilled = false;

    private void Awake ()
    {
        this.image = GetComponent<RawImage>();
        this.rectTransform = GetComponent<RectTransform>();
    }


    public void SetIndex (int newIndex, int agentCount, float transitionDuration = 0)
    {
        if(lastIndex == newIndex)
        {
            if(newIndex == -1)
            {
                SetOpacity(0);
            }
            return;
        }

        if(newIndex == -1)
        {
            if(lastIndex == 0)
            {
                Scale(0, transitionDuration);
                Shift(turnInterface.GetIconIndexPosition(-1), transitionDuration);
                Fade(0, transitionDuration);
            }
            else
            {
                SetOpacity(0);
            }
            

            lastIndex = newIndex;
            return;
        }

        if(lastIndex == -1)
        {
            SetPosition(turnInterface.GetIconIndexPosition(agentCount));
            SetOpacity(0);
        }

        if(lastIndex == 0)
        {
            StartCoroutine(WrapAround(newIndex, transitionDuration));
        }
        else
        {
            Fade(1, transitionDuration);
            Scale(newIndex == 0 ? 2 : 1, transitionDuration);
            Shift(turnInterface.GetIconIndexPosition(newIndex), transitionDuration);
        }

        lastIndex = newIndex;
    }

    public void Initialize (Agent agent, TurnInterface turnInterface)
    {
        this.agent = agent;
        this.turnInterface = turnInterface;
        image.color = new Color(Random.value, Random.value, Random.value, 1);
        
    }

    private void Update ()
    {
        if(!tokenFilled)
        {
            if(agent.Character != null)
            {
                token.AssignPortrait(agent.Character, agent.Friendly);
            }
            tokenFilled = true;
        }

        if(agent != null && agent.Character.HP <= 0)
        {
            SetOpacity(0);
        }
    }


    public IEnumerator WrapAround (int newIndex, float duration)
    {
        float t = duration / 2;

        Fade(0, t);
        Shift(turnInterface.GetIconIndexPosition(-1), t);
        Scale(0, t);

        while(fadeInProgress || shiftInProgress)
        {
            yield return new WaitForEndOfFrame();
        }

        SetPosition(turnInterface.GetIconIndexPosition(newIndex + 1));
        Fade(1, t);
        Shift(turnInterface.GetIconIndexPosition(newIndex), t);
        Scale(1, t);
    }

    public void Fade (float endOpacity, float duration)
    {
        StartCoroutine(FadeCoroutine(endOpacity, duration));
    }

    private IEnumerator FadeCoroutine (float endOpacity, float duration)
    {
        while(fadeInProgress)
        {
            fadeSpeed = 10000;
            yield return new WaitForEndOfFrame();
        }
        fadeSpeed = 1;

        fadeInProgress = true;

        float startOpacity = image.color.a;
        float time = 0;

        while(time <= duration)
        {
            float t = time / duration;
            float t2 = Mathf.SmoothStep(0, 1, t);

            SetOpacity(Mathf.Lerp(startOpacity, endOpacity, t2));
            yield return new WaitForEndOfFrame();
            time = time == duration
                    ? duration + 1
                    : Mathf.Min(time + Time.unscaledDeltaTime * fadeSpeed, duration);
        }
        fadeInProgress = false;
    }


    private void SetOpacity (float value)
    {
        Color color = image.color;
        color.a = value;
        image.color = color;
        opacity = value;
        if(value < 0.1f)
        {
            token.gameObject.SetActive(false);
        }
        else
        {
            token.gameObject.SetActive(true);
        }
    }

    public void SetPosition (Vector2 position)
    {
        rectTransform.anchorMin = position;
        rectTransform.anchorMax = position;
        //rectTransform.anchoredPosition = Vector2.zero;
    }

    public void Shift (Vector2 position, float duration)
    {
        StartCoroutine(ShiftCoroutine(rectTransform.anchorMin, position, duration));
    }

    private IEnumerator ShiftCoroutine (Vector2 startPos, Vector2 endPos, float duration)
    {
        while(shiftInProgress)
        {
            shiftSpeed = 10000;
            yield return new WaitForEndOfFrame();
        }
        shiftSpeed = 1;

        shiftInProgress = true;

        float time = 0;

        while(time <= duration)
        {
            float t = time/ duration;

            float t2 = Mathf.SmoothStep(0, 1, t);

            SetPosition(Vector2.Lerp(startPos, endPos, t2));

            yield return new WaitForEndOfFrame();

            time = time == duration
        ? duration + 1
        : Mathf.Min(time + Time.unscaledDeltaTime * shiftSpeed, duration);
        }
        shiftInProgress = false;
    }


    public void Scale (float value, float duration)
    {
        StartCoroutine(ScaleCoroutine(rectTransform.localScale.x, value, duration));
    }

    private IEnumerator ScaleCoroutine (float startScale, float endScale, float duration)
    {
        while(scaleInProgress)
        {
            scaleSpeed = 10000;
            yield return new WaitForEndOfFrame();
        }
        scaleSpeed = 1;

        scaleInProgress = true;

        float time = 0;

        while(time <= duration)
        {
            float t = time / duration;

            float scale = Mathf.SmoothStep(startScale, endScale, t);

            rectTransform.localScale = new Vector3(scale, scale, scale);

            yield return new WaitForEndOfFrame();

            time = time == duration
                    ? duration + 1
                    : Mathf.Min(time + Time.unscaledDeltaTime * scaleSpeed, duration);
        }
        scaleInProgress = false;
    }
}
