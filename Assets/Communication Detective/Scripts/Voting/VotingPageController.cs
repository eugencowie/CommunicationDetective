using UnityEngine;

public class VotingPageController : MonoBehaviour
{
    public GameObject PanelLeft;
    public GameObject PanelRight;

    public void Left()
    {
        if (PanelLeft != null)
        {
            PanelLeft.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    public void Right()
    {
        if (PanelRight != null)
        {
            PanelRight.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
