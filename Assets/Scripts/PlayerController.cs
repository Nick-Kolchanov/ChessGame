using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private ChessBoard chessBoard;
    [SerializeField] private Transform whitePos;
    [SerializeField] private Transform blackPos;
    [SerializeField] private float timeForSlerp = 0.2f;

    private float startTime;
    Vector3 whiteRelCenter;
    Vector3 blackRelCenter;
    Vector3 center;
    float fracComplete;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            ThrowARay();
        }
    }

    void ThrowARay()
    {
        Ray ray = gameObject.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform == chessBoard.GetComponent<Transform>())
                chessBoard.InputCell(hit.point);
        }
    }

    public void RotateCamera(bool isWhite)
    {
        startTime = Time.time;
        
        if (!isWhite)
        {
            center = (whitePos.position + blackPos.position) * 0.5F;
            center -= new Vector3(1, 0, 0);
            whiteRelCenter = whitePos.position - center;
            blackRelCenter = blackPos.position - center;
            InvokeRepeating("RotateToBlack", 0, 0.02f);
        }
        else
        {
            center = (whitePos.position + blackPos.position) * 0.5F;
            center -= new Vector3(0, 0, -1);
            whiteRelCenter = whitePos.position - center;
            blackRelCenter = blackPos.position - center;
            InvokeRepeating("RotateToWhite", 0, 0.02f);
        }
    }

    private void RotateToBlack()
    {
        fracComplete = (Time.time - startTime) / timeForSlerp;
        transform.position = Vector3.Slerp(whiteRelCenter, blackRelCenter, fracComplete);
        transform.position += center;

        transform.rotation = Quaternion.Lerp(whitePos.rotation, blackPos.rotation, fracComplete);

        if(transform.position == blackPos.position)
        {
            CancelInvoke();
        }
    }

    private void RotateToWhite()
    {
        fracComplete = (Time.time - startTime) / timeForSlerp;
        transform.position = Vector3.Slerp(blackRelCenter, whiteRelCenter, fracComplete);
        transform.position += center;

        transform.rotation = Quaternion.Slerp(blackPos.rotation, whitePos.rotation, fracComplete);

        if (transform.position == whitePos.position)
        {
            CancelInvoke();
        }
    }
}
