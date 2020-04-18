using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lurker : MonoBehaviour
{
    public List<SpriteRenderer> blushes;
    public Color altColor;
    public Face face;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale *= Random.Range(0.75f, 1.2f);

        if(Random.value < 0.5f)
        {
            blushes.ForEach(b => b.color = altColor);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Hit by " + collision.gameObject.name);

        if(collision.gameObject.tag == "Poop")
        {
            face.Emote(Face.Emotion.Sad, Face.Emotion.Default, 2f);
        }

        if (collision.gameObject.tag == "Bird")
        {
            face.Emote(Face.Emotion.Shocked, Face.Emotion.Default, 3f);
        }
    }
}
