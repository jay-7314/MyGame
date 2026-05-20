using UnityEngine;

public class SkyLoop : MonoBehaviour
{
    [SerializeField] float speed = 3f;      //이미지가 이동하는 속도
    [SerializeField] float posValue;        //이미지의 너비
    /*
     * 너비 계산하는법 
     * -> rawImage가 아닌 sprite형식이라서 배율로 하기 때문에 정확한 사이즈를 모름
     * -> project에서 원본 이미지를 선택하면 이미지 아래에 가로 사이즈가 적혀있음 ex) 384x288
     * -> 여기에서 배율이 5배율(이건 내가 사이즈 맞추기 위해서 한 값), PPU(Pixel Per Unit)이 100이기 때문에
     * 384*5/100 = 19.2 따라서 19정도 설정하면 된다.(19.2로 했더니 미묘하게 끊기는 부분이 있다.)
     */

    Vector2 startPos;                       //현재 이미지의 처음 위치
    float newPos;                           //새로운 위치

    void Start()
    {
        startPos = transform.position;      //위치 파악
    }

    void Update()
    {
        newPos = Mathf.Repeat(Time.time * speed, posValue);             //새로운위치의 최대값을 본다. 왼쪽값이 커지면 왼쪽값을, 오른쪽 값이 커지면 오른쪽값을 가져간다.
        transform.position = startPos + Vector2.left * newPos;          //위치를 다시 잡는데, 처음 위치 + 왼쪽으로 가면서 * 새로운 위치의 값을 곱한다.
    }
}
