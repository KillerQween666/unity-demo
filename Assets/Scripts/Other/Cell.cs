using FTRuntime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// ��Ԫ��ű�������ֲ����ֲ�������Ƴ�ֲ��Լ����hoverʱ��Ԥ��Ч��
public class Cell : MonoBehaviour {

    // ��ǰ��Ԫ������ֲ��ֲ�null��ʾ��ֲ�
    public Plant currentPlant;
    // ���hoverʱ��ֲ��Ԥ��ʵ������������ʾ����ʵ����Ч��
    private Plant plantPreview;

    // ��Ԫ�����¼�����ֲֲ��/ʹ�ò��ӣ���HandManager����
    public void OnClick() {
        // �����ٿ��ܴ��ڵ�Ԥ��ֲ��������
        if (plantPreview != null) Destroy(plantPreview.gameObject);
        // ֪ͨHandManager�������߼�����ֲ�������
        HandManager.Instance.OnCellClick(this);
    }

    // �����뵥Ԫ��ʱ��������ʾֲ��Ԥ��/��������ֲ�
    public void OnPointerEnter(BaseEventData data) {
        // ����Ԫ����ֲ����ѡ���˲��ӣ���ֲ���������ʾ�ɲ�����
        if (currentPlant != null && HandManager.Instance.shovel.activeSelf) {
            currentPlant.animator.SetBool("isBright", true);
        }

        // ��û��ѡ��ֲ���Ԫ������ֲ�����ʾԤ��
        if (HandManager.Instance.currentPlant == null || currentPlant != null) return;

        // ����ֲ��Ԥ��ʵ�������Ƶ�ǰѡ�е�ֲ�
        plantPreview = Instantiate(HandManager.Instance.currentPlant);
        plantPreview.GetComponent<Collider2D>().enabled = false; // ������ײ�壬����Ӱ�콻��

        // ����Ԥ��ֲ�����ʾЧ������͸����alpha 0.6�����㼶���ͣ������ڵ�����UI��
        SpriteRenderer[] sprites = plantPreview.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var sprite in sprites) {
            sprite.color = new Color(1, 1, 1, 0.6f);
            sprite.sortingOrder -= 100;
        }

        // ��Ԥ��ֲ����浥Ԫ��λ��
        plantPreview.transform.position = transform.position;
    }

    // ����뿪��Ԫ��ʱ����������Ԥ��/ȡ��ֲ�������
    public void OnPointerExit(BaseEventData data) {
        // ����Ԫ����ֲ����ѡ���˲��ӣ�ȡ��ֲ�����
        if (currentPlant != null && HandManager.Instance.shovel.activeSelf) {
            currentPlant.animator.SetBool("isBright", false);
        }

        // ����Ԥ��ֲ��������
        if (plantPreview != null) {
            Destroy(plantPreview.gameObject);
        }
    }

    // �Ƴ���Ԫ���ϵ�ֲ����ӹ��ܣ�
    public void SubPlant() {
        // ��ֲ���δѡ�в��ӣ���ִ���Ƴ�
        if (currentPlant == null || !HandManager.Instance.shovel.activeSelf) return;

        HandManager.Instance.ReturnShovel(); // �黹���ӣ�ȡ��ѡ��״̬��
        AudioManager.Instance.PlayClip(Random.value > 0.5f ? Config.plant : Config.plant2); // �����Ƴ���Ч

        Destroy(currentPlant.gameObject); // ����ֲ��ʵ��
        currentPlant = null; // ��յ�ǰֲ������
    }

    // �ڵ�Ԫ������ֲֲ�ѡ��ֲ�������Ԫ�񴥷���
    public void AddPlant() {
        // ����ֲ���δѡ��ֲ���ִ����ֲ
        if (currentPlant != null || HandManager.Instance.currentPlant == null) return;

        // ����ֲ����ֲλ�ã�Z����Ϊ-2��ȷ����ʾ����ȷ�㼶��
        Vector3 position = transform.position;
        position.z = -2;

        // ����ֲ��ʵ������Ԫ��λ��
        currentPlant = Instantiate(HandManager.Instance.currentPlant, position, Quaternion.identity);

        // ����ֲ����Ⱦ�㼶Ϊ"Game"������Ϸ������������һ�£�
        SpriteRenderer[] sprites = currentPlant.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var sprite in sprites) {
            sprite.sortingLayerName = "Game";
        }

        // ����ֲ�����Ϊ����״̬����ֲ�￪ʼ������
        currentPlant.TransitionToEnable();
    }
}