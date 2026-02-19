using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Emergency : MonoBehaviour
{
    Canvas canvas;
    public Image img;

    List<SimpleOrc> orcs = new List<SimpleOrc>();

    private void Start()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        GameObject a = Instantiate(new GameObject());
        a.AddComponent<Image>();
        a.AddComponent<RectTransform>();
        a.GetComponent<RectTransform>().parent = canvas.transform;
        img = a.GetComponent<Image>();
        img.sprite = Resources.Load<Sprite>("danger");

        GameManager.instance.emergencies.Add(this);

        for (int i = 0; i < GameManager.instance.orcs.Count; i++)
        {
            if (Vector3.Distance(GameManager.instance.orcs[i].body.position, transform.position) < 200)
            {
                try
                {
                    (GameManager.instance.orcs[i] as SimpleOrc).emergency = this;
                    (GameManager.instance.orcs[i] as SimpleOrc).ChangeState(SimpleOrc.states.emergency);
                    orcs.Add((GameManager.instance.orcs[i] as SimpleOrc));
                }
                catch { }
            }
        }
    }

    float cd = 0;
    void Update()
    {
        SetPosition();

        cd += Time.deltaTime;

        if(cd > 20)
        {
            for (int i = 0; i < orcs.Count; i++)
            {
                orcs[i].ChangeState(SimpleOrc.states.wanderInVillage);
            }

            Destroy(img.gameObject);
            GameManager.instance.emergencies.Remove(this);
            Destroy(gameObject);
        }
    }

    void SetPosition()
    {
        float minX = img.GetPixelAdjustedRect().width / 2;
        float maxX = Screen.width - minX;

        float minY = img.GetPixelAdjustedRect().height / 2;
        float maxY = Screen.height - minY;

        Vector2 pos = Camera.main.WorldToScreenPoint(transform.position);
        if (Vector3.Dot((transform.position - Camera.main.transform.position), Camera.main.transform.forward) < 0)
        {
            if (pos.x < Screen.width / 2)
                pos.x = maxX;
            else
                pos.x = minX;
        }

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        img.transform.position = pos;
    }
}
