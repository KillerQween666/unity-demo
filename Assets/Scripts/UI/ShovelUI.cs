using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// ����UI�������Ƴ�ֲ�
public class ShovelUI : MonoBehaviour {

    public Image shovelImage; // ����ͼ��

    private void Start() {
        Show(); // ��ʼ��ʾ
    }

    // ��ʾ����ͼ��
    public void Show() {
        shovelImage.enabled = true;
    }

    // ���ز���ͼ��
    public void Hide() {
        shovelImage.enabled = false;
    }

    // ���̧��ʱ���Ƴ����λ�õ�ֲ�UI�¼����ã�
    public void OnPointerUp() {
        // ת�����λ�õ���������
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0;

        // ���߼�����ĵ�Ԫ��
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero);

        if (hit && hit.collider.CompareTag("Cell")) {
            // ���õ�Ԫ����Ƴ�ֲ�﷽��
            Cell cell = hit.collider.GetComponent<Cell>();
            cell.SubPlant();
        }
    }
}