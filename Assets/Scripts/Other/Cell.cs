using FTRuntime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// 单元格脚本，负责植物种植、铲子移除植物，以及鼠标hover时的预览效果
public class Cell : MonoBehaviour {

    // 当前单元格上种植的植物（null表示无植物）
    public Plant currentPlant;
    // 鼠标hover时的植物预览实例（仅用于显示，不实际生效）
    private Plant plantPreview;

    public bool isHole = false;

    public float holeTime = 10;
    private float holeTimer;

    public GameObject nightHole;
    public GameObject nightHole2;

    public bool canSpawnBrave;
    public bool canSpawnWaterZombie = false;

    public GameObject bravePrefab;
    public bool isBrave = false;

    private bool isWaterLand = false;

    private bool isHaveIce;
    public float iceLiveTime = 30;
    private float iceLiveTimer = 0;

    public Plant topPlant;

    public bool isWater = false;

    public int row;

    public Transform waterLandTransform;

    private Collider2D coll;

    private void Awake() {
        coll = GetComponent<Collider2D>();
        coll.enabled = false;
    }

    public void OpenCollider2D() {
        coll.enabled = true;
    }

    private void Update() {
        if (isHole) {

            holeTimer += Time.deltaTime;
            if (holeTimer >= holeTime * 0.5 && !nightHole2.activeSelf) {
                nightHole.SetActive(false);
                nightHole2.SetActive(true);
            }
            if (holeTimer >= holeTime) {
                nightHole2.SetActive(false);
                holeTimer = 0;
                isHole = false;
            }
        }

        if (isHaveIce) {
            iceLiveTimer += Time.deltaTime;
            if (iceLiveTimer > iceLiveTime) {
                iceLiveTimer = 0;
                
                isHaveIce = false;
            }
        }
    }

    // 单元格点击事件（种植植物/使用铲子，由HandManager处理）
    public void OnClick() {
        // 先销毁可能存在的预览植物，避免残留
        if (plantPreview != null) Destroy(plantPreview.gameObject);
        // 通知HandManager处理点击逻辑（种植或铲除）
        HandManager.Instance.OnCellClick(this);
    }

    // 鼠标进入单元格时触发（显示植物预览/高亮已有植物）
    public void OnPointerEnter(BaseEventData data) {
        if (isHole || isHaveIce) return;

        // 若单元格有植物且选中了铲子，让植物高亮（提示可铲除）
        if (HandManager.Instance.shovel.activeSelf) {
            if (topPlant != null) {
                topPlant.isBrighten = true;
                topPlant.PlayBright();
            } else if (currentPlant != null) {
                currentPlant.isBrighten = true;
                currentPlant.PlayBright();
            }
        }

        if (HandManager.Instance.currentPlant == null) return;

        if (!HandManager.Instance.currentPlant.isCoverPlant && isWaterLand == false && currentPlant != null) return;

        if (HandManager.Instance.currentPlant.isCoverPlant) {
            if (topPlant != null) {
                if (topPlant.plantType != HandManager.Instance.currentPlant.coverPlantType) return;
            } else {
                if (currentPlant == null || currentPlant.plantType != HandManager.Instance.currentPlant.coverPlantType) return;
            }
        } else {
            if (isWaterLand) {
                if (topPlant != null) return;
                if (HandManager.Instance.currentPlant.isLandPlant || HandManager.Instance.currentPlant.isWaterPlant) return;
            }
            else {
                if (isWater && !HandManager.Instance.currentPlant.isWaterPlant) return;
                if (HandManager.Instance.currentPlant.isWaterPlant && !isWater) return;
            }
        }

        if (isBrave && HandManager.Instance.currentPlant.plantType != PlantType.Gravebuster) return;
        if (HandManager.Instance.currentPlant.plantType == PlantType.Gravebuster && !isBrave) return;

        // 生成植物预览实例（复制当前选中的植物）
        plantPreview = Instantiate(HandManager.Instance.currentPlant);
        plantPreview.TransitionToDisable();     // 禁用功能，避免影响交互

        // 调整预览植物的显示效果：半透明（alpha 0.6）、层级降低（避免遮挡其他UI）
        SpriteRenderer[] sprites = plantPreview.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var sprite in sprites) {
            sprite.color = new Color(1, 1, 1, 0.6f);
            sprite.sortingOrder -= 100;
        }

        // 让预览植物跟随单元格位置
        if (isWaterLand) {
            plantPreview.transform.position = waterLandTransform.position;
        } else {
            plantPreview.transform.position = transform.position;
        }
            
    }

    // 鼠标离开单元格时触发（销毁预览/取消植物高亮）
    public void OnPointerExit(BaseEventData data) {

        // 若单元格有植物且选中了铲子，取消植物高亮
        if (HandManager.Instance.shovel.activeSelf) {
            if (topPlant != null) {
                topPlant.isBrighten = false;
                topPlant.StopBright();
            } else if (currentPlant != null) {
                currentPlant.isBrighten = false;
                currentPlant.StopBright();
            }
        }

        // 销毁预览植物，避免残留
        if (plantPreview != null) {
            Destroy(plantPreview.gameObject);
        }
    }

    // 移除单元格上的植物（铲子功能）
    public void SubPlant() {
        // 无植物或未选中铲子，不执行移除
        if (currentPlant == null && topPlant == null) return;

        if (topPlant != null) {
            topPlant.Dead();
            topPlant = null;
        } else if (currentPlant != null) {
            currentPlant.Dead(); // 销毁植物实例
            currentPlant = null; // 清空当前植物引用
        }

        HandManager.Instance.ReturnShovel(); // 归还铲子（取消选中状态）
        AudioManager.Instance.PlayClip(Random.value > 0.5f ? Config.plant : Config.plant2); // 播放移除音效

    }

    // 在单元格上种植植物（选中植物后点击单元格触发）
    public bool AddPlant() {
        Plant plant = HandManager.Instance.currentPlant;

        if (plant == null || isHole || isHaveIce) return false;

        if (!plant.isCoverPlant && isWaterLand == false && currentPlant != null) return false;

        if (plant.isCoverPlant) {
            if (topPlant != null) {
                if (topPlant == null || topPlant.plantType != plant.coverPlantType) return false;
            } else {
                if (currentPlant == null || currentPlant.plantType != plant.coverPlantType) return false;
            }  
        } else {
            if (isWaterLand) {
                if (plant.isLandPlant || plant.isWaterPlant || topPlant != null) return false;
            }
            else {
                if (isWater && !plant.isWaterPlant) return false;
                if (plant.isWaterPlant && !isWater) return false;
            }
        }

        if (plant.plantType == PlantType.Gravebuster && !isBrave ) return false;
        if (isBrave && plant.plantType != PlantType.Gravebuster) return false;

        // 调整植物种植位置（Z轴设为-2，确保显示在正确层级）
        Vector3 position = transform.position;
        position.z = -2;

        // 生成植物实例到单元格位置
        if (plant.isCoverPlant) {
            if (topPlant != null) {
                topPlant.Dead();
                topPlant = null;
            } else {
                currentPlant.Dead();
                currentPlant = null;
            }
        }

        if (isWaterLand) {
            if (topPlant != null) return false;
            topPlant = Instantiate(HandManager.Instance.currentPlant, waterLandTransform.position, Quaternion.identity);
            topPlant.selfCell = this;
        } else {
            currentPlant = Instantiate(HandManager.Instance.currentPlant, position, Quaternion.identity);
            currentPlant.selfCell = this;
        }   
        
        if (topPlant != null) {
            // 调整植物渲染层级为"Game"（与游戏场景其他物体一致）
            SpriteRenderer[] sprites = topPlant.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (var sprite in sprites) {
                sprite.sortingLayerName = ZombieManager.Instance.layerNames[row];
            }

            // 激活植物（设置为启用状态，让植物开始工作）
            topPlant.TransitionToEnable();
        } else {
            // 调整植物渲染层级为"Game"（与游戏场景其他物体一致）
            SpriteRenderer[] sprites = currentPlant.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (var sprite in sprites) {
                sprite.sortingLayerName = ZombieManager.Instance.layerNames[row];
            }

            // 激活植物（设置为启用状态，让植物开始工作）
            currentPlant.TransitionToEnable();
        }

        return true;
    }

    public void StartIce() {
        isHaveIce = true;
        if (topPlant != null) SubPlant();
        if (currentPlant != null) SubPlant();
        iceLiveTimer = 0;
    }

    public void EndIce() {
        isHaveIce = false;
        iceLiveTimer = 0;
    }

    public void StartHole() {
        isHole = true;
        nightHole.SetActive(true);
    }

    public void BadTombstone() {
        bravePrefab.GetComponent<Grave>().BadGrave();
    }

    public void GoodTombstone() {
        bravePrefab.GetComponent<Grave>().GoodGrave();
    }

    public void BusterTombstone() {
        isBrave = false;
        Destroy(bravePrefab);
    }

    public void OpenWaterLand() {
        isWaterLand = true;
    }

    public void CloseWaterLand() {
        isWaterLand = false;
    }
}