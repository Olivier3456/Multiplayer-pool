using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUIManager : MonoBehaviour
{
    [SerializeField] List<Image> redBalls;
    [SerializeField] List<Image> yellowBalls;

    [SerializeField] Image whiteBall;

    [SerializeField] TextMeshProUGUI victoryText;

    public int nextRedBallToGray = 0;
    public int nextYellowBallToGray = 0;

    public void RedBallIHole()
    {
        if (nextRedBallToGray < redBalls.Count)
        {
            GrayTheBall(redBalls[nextRedBallToGray]);

            StartCoroutine(WhiteBallEffect(redBalls[nextRedBallToGray].rectTransform.position));

            nextRedBallToGray++;
        }
        

        if (nextRedBallToGray == redBalls.Count)
        {
            StartCoroutine(VictoryTextEffect("Red"));
        }
    }

    public void YellowBallIHole()
    {
        if (nextYellowBallToGray < yellowBalls.Count)
        {
            GrayTheBall(yellowBalls[nextYellowBallToGray]);

            StartCoroutine(WhiteBallEffect(yellowBalls[nextYellowBallToGray].rectTransform.position));

            nextYellowBallToGray++;
        }
        
        if (nextYellowBallToGray == yellowBalls.Count)
        {
            StartCoroutine(VictoryTextEffect("Yellow"));
        }

    }

    private void GrayTheBall(Image ball)
    {
        ball.color = new Color(0, 0, 0, 0.35f);
    }


    IEnumerator VictoryTextEffect(string winnerName)
    {
        victoryText.text = winnerName + " win!";
        victoryText.rectTransform.position = new Vector3 (0, 0, 0);
        victoryText.gameObject.SetActive(true);

        while (victoryText.rectTransform.position.y < 300)
        {
            victoryText.rectTransform.position = new Vector3(0, victoryText.rectTransform.position.y + Time.deltaTime, 0);
            yield return null;
        }


    }


    IEnumerator WhiteBallEffect(Vector2 position)
    {
        whiteBall.rectTransform.position = position;
        float a = 1;
        Vector2 scale = new Vector2(4, 4);

        whiteBall.color = new Color(0, 0, 0, 1);
        whiteBall.rectTransform.localScale = scale;

        while (a > 0)
        {
            float speed = 5;

            a -= Time.deltaTime * speed;
            scale = new Vector2(scale.x - Time.deltaTime * speed * 2, scale.y - Time.deltaTime * speed * 2);

            whiteBall.color = new Color(0, 0, 0, a);
            whiteBall.rectTransform.localScale = scale;
            yield return null;
        }
    }
}
