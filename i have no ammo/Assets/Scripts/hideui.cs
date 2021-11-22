using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class hideui : MonoBehaviour
{
    //image uis
    public List<Image> uiImages = new List<Image>();
    private List<Color> imageColors = new List<Color>();

    //text uis
    public List<Text> uiText = new List<Text>();
    private List<Color> textColors = new List<Color>();

    private const float imageOpacity = .25f;

    private void Start()
    {
        //image color set up
        foreach (Image image in uiImages)
        {
            imageColors.Add(image.color);
        }

        //text color set up
        foreach (Text text in uiText)
        {
            textColors.Add(text.color);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //lower opacity of images
        for(int i = 0; i < uiImages.Count; i++)
        {
            Color c = imageColors[i];
            c.a = imageOpacity;
            uiImages[i].color = c;
        }

        //lower opacity of text
        for (int i = 0; i < uiText.Count; i++)
        {
            Color c = textColors[i];
            c.a = imageOpacity;
            uiText[i].color = c;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //make image colors go back to normal
        for (int i = 0; i < uiImages.Count; i++)
        {
            uiImages[i].color = imageColors[i];
        }

        //make text colors go back to normal
        for (int i = 0; i < uiText.Count; i++)
        {
            uiText[i].color = textColors[i];
        }
    }
}
