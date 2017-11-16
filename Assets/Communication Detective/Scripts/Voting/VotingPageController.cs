using UnityEngine;

public class VotingPageController : MonoBehaviour
{
    public GameObject PanelLeft;
    public GameObject PanelRight;

    public void Left()
    {
        if (PanelLeft != null)
        {
            gameObject.SetActive(false);
            PanelLeft.SetActive(true);
        }
    }

    public void Right()
    {
        if (PanelRight != null)
        {
            gameObject.SetActive(false);
            PanelRight.SetActive(true);
        }
    }
}
