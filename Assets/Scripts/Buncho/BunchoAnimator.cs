using System;
using System.Collections;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public enum MoveDirection
{
    Center,
    Right,
    Left
}

public class BunchoAnimator : MonoBehaviour
{
    public const int WalkMoveAmount = 40;
    public Sprite openEyeSprite;
    public Sprite closeEyeSprite;
    public Transform footLeft;
    public Transform footRight;
    public Transform bodyRoot;
    public Transform head;
    public Transform tailRoot;
    public Transform tail2;
    public Image eyeLeft;
    public Image eyeRight;
    private Transform[] _offsets = Array.Empty<Transform>();

    private void Awake()
    {
        _offsets = new Transform[]
        {
            bodyRoot,
            head,
            tailRoot,
            tail2
        };
    }

    public void Reset()
    {
        foreach (var offset in _offsets)
        {
            offset.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
            offset.GetComponent<RectTransform>().rotation = Quaternion.identity;
        }
    }


    public IEnumerator Mochi()
    {
        Reset();
        bodyRoot.GetComponent<RectTransform>().anchoredPosition3D = Vector3.down * 100;
        tailRoot.GetComponent<RectTransform>().anchoredPosition3D = Vector3.down * 100;
        tail2.GetComponent<RectTransform>().anchoredPosition3D = Vector3.down * 100;
        head.GetComponent<RectTransform>().anchoredPosition3D = Vector3.up * 100;
        yield break;
    }
    
    public IEnumerator Stand()
    {
        Reset();
        tailRoot.GetComponent<RectTransform>().anchoredPosition3D = Vector3.down * 50;
        head.GetComponent<RectTransform>().anchoredPosition3D = Vector3.up * 250;
        yield break;
    }
    
    public IEnumerator MochiKubiKashige()
    {
        yield return Mochi();
        head.GetComponent<RectTransform>().anchoredPosition3D += Vector3.right * 120;
        head.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, -60);
    }

    public IEnumerator Walk(MoveDirection direction, int moveAmount = WalkMoveAmount)
    {
        var moveAmountVector = Vector3.right * (direction == MoveDirection.Right ? moveAmount : -moveAmount);
        footRight.GetComponent<RectTransform>().anchoredPosition3D += moveAmountVector;
        yield return new WaitForSeconds(0.1f);
        bodyRoot.GetComponent<RectTransform>().anchoredPosition3D += moveAmountVector;
        yield return new WaitForSeconds(0.1f);
        footLeft.GetComponent<RectTransform>().anchoredPosition3D += moveAmountVector;
        transform.position += moveAmountVector;
        footRight.GetComponent<RectTransform>().anchoredPosition3D -= moveAmountVector;
        footLeft.GetComponent<RectTransform>().anchoredPosition3D -= moveAmountVector;
        bodyRoot.GetComponent<RectTransform>().anchoredPosition3D -= moveAmountVector;
    }
    
    public IEnumerator JumpStandby(MoveDirection direction)
    {
        Reset();
        bodyRoot.GetComponent<RectTransform>().anchoredPosition3D = Vector3.down * 100;

        switch (direction)
        {
            case MoveDirection.Center:
                break;
            case MoveDirection.Right:
                head.GetComponent<RectTransform>().anchoredPosition3D = Vector3.right * 50;
                break;
            case MoveDirection.Left:
                head.GetComponent<RectTransform>().anchoredPosition3D = Vector3.left * 50;
                break;
        }

        yield break;
    }

    public IEnumerator CloseEye()
    {
        eyeLeft.sprite = closeEyeSprite;
        eyeRight.sprite = closeEyeSprite;
        yield break;
    }

    public IEnumerator OpenEye()
    {
        eyeLeft.sprite = openEyeSprite;
        eyeRight.sprite = openEyeSprite;
        yield break;
    }
    
    
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            StartCoroutine(Mochi());
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            StartCoroutine(MochiKubiKashige());
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            StartCoroutine(Stand());
        }
        if (Input.GetKeyUp(KeyCode.F))
        {
            StartCoroutine(JumpStandby(MoveDirection.Center));
        }
        if (Input.GetKeyUp(KeyCode.G))
        {
            StartCoroutine(JumpStandby(MoveDirection.Right));
        }
        if (Input.GetKeyUp(KeyCode.H))
        {
            StartCoroutine(JumpStandby(MoveDirection.Left));
        }
        if (Input.GetKeyUp(KeyCode.X))
        {
            StartCoroutine(Walk(MoveDirection.Right));
        }
        if (Input.GetKeyUp(KeyCode.Z))
        {
            StartCoroutine(Walk(MoveDirection.Left));
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            StartCoroutine(CloseEye());
        }
        if (Input.GetKeyUp(KeyCode.V))
        {
            StartCoroutine(OpenEye());
        }
    }
}
