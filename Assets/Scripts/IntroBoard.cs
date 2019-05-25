using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroBoard : MonoBehaviour
{
    public bool AnimateBoard = true;
    public float BoardScaleFactor = 0.8f;
    float tileSize;
    float knownScreenWidth = 0f;
    float knownScreenHeight = 0f;
    Animator animator;

    private bool HasNewScreenSize { get { return knownScreenHeight != Screen.height || knownScreenWidth != Screen.width; } }
    
    private void Awake()
    {
        CalculateTileSize();
        SetGridParams();
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if(AnimateBoard)
        {
            PlayIntroAnimation();
        }
        else
        {
            FinishIntro();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(HasNewScreenSize)
        {
            CalculateTileSize();
            SetGridParams();
        }
    }

    private void PlayIntroAnimation()
    {
        //animator.Play("IntroAnimation");
    }
    private void FinishIntro()
    {
        //animator.Play("IntroComplete");
    }
    
    private void CalculateTileSize()
    {
        knownScreenHeight = Screen.height;
        knownScreenWidth = Screen.width;
        float sqScreenSize = Mathf.Min(knownScreenWidth, knownScreenHeight) * BoardScaleFactor;
        tileSize = sqScreenSize / 3;
    }
    private void SetGridParams()
    {
        GridLayoutGroup glg = GetComponent<GridLayoutGroup>();
        glg.constraintCount = 3;
        glg.cellSize = new Vector2(tileSize, tileSize);
        glg.spacing = glg.cellSize / 16;
        glg.padding.top = (int)(knownScreenHeight / 6);
    }
}
