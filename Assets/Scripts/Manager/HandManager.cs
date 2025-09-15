using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �ֲ�������������ֲ�������ꡢ��ֲֲ����Ӳ����Ⱥ��Ľ����߼�
public class HandManager : MonoBehaviour {

    // ����ʵ����ȫ��Ψһ���ʵ�
    public static HandManager Instance { get; private set; }

    // �洢ֲ���������ӦԤ�����ӳ�䣨��Resources���أ����ٻ�ȡԤ���壩
    private Dictionary<PlantType, Plant> plantPrefabs = new Dictionary<PlantType, Plant>();

    // ��ǰѡ�е�ֲ�������꣬δ��ֲ��
    public Plant currentPlant;

    // ��ǰѡ��ֲ���Ӧ�Ŀ��ƣ����ں�����ȴ�Ȳ�����
    private Card card;

    // ��ǰѡ�еĲ��ӣ�������꣬δʹ�ã�
    public GameObject shovel;

    // ��ʼ���������ռ�����ֲ��Ԥ����
    private void Awake() {
        Instance = this;
        CollectAllPlantPrefabs();
    }

    // ÿ֡���£���ѡ�е�ֲ��/���Ӹ������
    private void Update() {
        FollowCursor();
    }

    // ��Resources/PlantsĿ¼�ռ�����ֲ��Ԥ���壬�����ֵ�
    private void CollectAllPlantPrefabs() {
        Plant[] plants = Resources.LoadAll<Plant>("Plants"); // ����ָ��·��������Plant����Ԥ����
        foreach (var plant in plants) {
            // �����ظ����ͬһ���͵�ֲ��Ԥ����
            if (!plantPrefabs.ContainsKey(plant.plantType)) {
                plantPrefabs.Add(plant.plantType, plant);
            }
        }
    }

    // �õ�ǰѡ�е�ֲ��/���Ӹ������λ��
    void FollowCursor() {
        // û��ѡ��ֲ��Ҳû��ѡ�в��ӣ�ֱ�ӷ���
        if (currentPlant == null && !shovel.activeSelf) return;

        // �������Ļ����ת��Ϊ�������꣨Z����Ϊ0����������������㼶��ͻ��
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0;

        // ѡ��ֲ������ֲ��������
        if (currentPlant != null) {
            currentPlant.transform.position = mouseWorldPosition;
            //currentPlant.transform.position = mouseWorldPosition;
        }
        // ѡ�в������ò��Ӹ������
        else if (shovel.activeSelf) {
            shovel.transform.position = mouseWorldPosition;
        }
    }

    // ���ѡ�е�ֲ��ӿ��Ƶ������������ֲ��Ԥ���岢������꣩
    public void AddPlant(PlantType plantType) {
        // ����ѡ�е�ֲ�����ӣ����ظ����
        if (currentPlant != null || shovel.activeSelf) return;

        AudioManager.Instance.PlayClip(Config.selectPlant); // ����ѡ��ֲ�����Ч
        card = CardManager.Instance.GetCardByPlantType(plantType); // ��ȡ��Ӧֲ��Ŀ���

        // ���ֵ��л�ȡֲ��Ԥ���壬����ʵ��
        if (plantPrefabs.TryGetValue(plantType, out Plant prefab)) {
            currentPlant = Instantiate(prefab);

            // ����ֲ��������Ⱦ���Ĳ㼶Ϊ"UI"��ȷ���������ʱ��ʾ�����ϲ�
            SpriteRenderer[] sprites = currentPlant.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (var sprite in sprites) {
                sprite.sortingLayerName = "UI";
            }

            // ��ʼ��Ϊ����״̬��δ��ֲʱ��������
            currentPlant.TransitionToDisable();
        }
    }

    // �����Ԫ��ʱִ�У���ֲֲ���ʹ�ò����Ƴ�ֲ�
    public void OnCellClick(Cell cell) {
        // ��Ԫ��û��ֲ��ʱ����ֲ��ǰѡ�е�ֲ��
        if (cell.currentPlant == null) {
            cell.AddPlant(); // ��Ԫ�����ֲ��

            // ��ֲ�ɹ��󣬴�������ȴ�������⡢���ٸ�������ֲ��
            if (cell.currentPlant != null) {
                card.TransitionToCooling(); // ���ƽ�����ȴ״̬
                SunManager.Instance.SubSun(card.needSunPoint); // �۳���ֲ��������
                AudioManager.Instance.PlayClip(Config.plant); // ������ֲ��Ч

                Destroy(currentPlant.gameObject); // ���ٸ�������ֲ��ʵ��
                currentPlant = null; // ��յ�ǰѡ��ֲ��
            }
        }

        // ��ѡ�еĲ���ʱ���Ƴ���Ԫ���е�ֲ��
        if (shovel.activeSelf) {
            cell.SubPlant();
        }
    }

    // �������UIʱִ�У�ѡ��/ȡ��ѡ�в��ӣ�
    public void OnShovelUIClick() {
        // ����ѡ�е�ֲ��޷�ѡ�в���
        if (currentPlant != null) return;

        // û��ѡ�в��ӣ���ʾ����ʵ����������꣬���ز���UI
        if (!shovel.activeSelf) {

            shovel.SetActive(true);
            AudioManager.Instance.PlayClip(Config.shovelClick); // ���Ų���ѡ����Ч
            UIManager.Instance.shovelUI.Hide();
        }
        // ����ѡ�еĲ��ӣ�ȡ��ѡ�У����ز���ʵ������ʾ����UI
        else {
            ReturnShovel();
            AudioManager.Instance.PlayClip(Random.value > 0.5f ? Config.tap : Config.tap2); // ����ȡ����Ч
        }
    }

    // �黹���ӣ�ȡ��ѡ�У��ָ���ʼ״̬��
    public void ReturnShovel() {
        shovel.SetActive(false); // ���ز���ʵ��
        UIManager.Instance.shovelUI.Show(); // ������ʾ����UI
    }
}