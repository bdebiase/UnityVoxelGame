using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PaintMode {Place, Break}
public enum BlockType {Cloth, Grass, Gravel, Sand, Snow, Stone, Wood, Glass, Metal}
public enum ControlType {Pro, FPS}
public class World : MonoBehaviour {
    //INSTANCE VARIABLES
    public int selectedSlot;
    public GameObject pauseMenu;
    public GameObject tooltip;
    public GameObject selectionGO;
    public GameObject inventorySlotGO;
    public GameObject inventory;
    public GameObject topPanel;
    public GameObject proControlsText;
    public GameObject fpsControlsText;
    public Dropdown controlsDropdown;
    public Slider sensitivitySlider;
    public Text sensitivityText;
    public PaintMode paintMode;
    public ControlType controlType;
    public Color paintColor;
    public int chunkSize;
    public List<GameObject> blocks = new List<GameObject>();
    public ParticleSystem blockBreakParticle;
    public bool paused;

    [Header("Block Type Audio")]
    public AudioClip[] clothAudio;
    public AudioClip[] grassAudio;
    public AudioClip[] gravelAudio;
    public AudioClip[] sandAudio;
    public AudioClip[] snowAudio;
    public AudioClip[] stoneAudio;
    public AudioClip[] woodAudio;
    public AudioClip[] glassPlaceAudio;
    public AudioClip[] glassBreakAudio;

    [Space]

    public float clothPitch;
    public float grassPitch;
    public float gravelPitch;
    public float sandPitch;
    public float snowPitch;
    public float stonePitch;
    public float woodPitch;
    public float glassPlacePitch;
    public float glassBreakPitch;
    public float metalPitch;

    //run at start of program
    private void Start() {
        //create cube of blocks
        for (int x = 0; x < chunkSize; x++) {
            for (int y = 0; y < chunkSize; y++) {
                for (int z = 0; z < chunkSize; z++) {
                    if (y < chunkSize - 1)
                        CreateBlock(new Vector3(x, y, z), Resources.Load("Blocks/Dirt") as GameObject);
                    else
                        CreateBlock(new Vector3(x, y, z), Resources.Load("Blocks/Grass") as GameObject);
                }
            }
        }
        UpdateAllBlocks();

        inventory = GameObject.Find("Inventory");
        tooltip = GameObject.Find("Tooltip");
        
        StartCoroutine(LoadSlots());
        inventory.SetActive(false);
        sensitivitySlider.value = PlayerPrefs.GetInt("Sensitivity");
        sensitivityText.text = PlayerPrefs.GetInt("Sensitivity").ToString();
    }

    //run every frame
    public void Update() {
        //set control type
        switch (controlType) {
            case ControlType.Pro:
            Camera.main.GetComponent<ProController>().enabled = true;
            Camera.main.GetComponent<FPSController>().enabled = false;
            break;
            default:
            Camera.main.GetComponent<FPSController>().enabled = true;
            Camera.main.GetComponent<ProController>().enabled = false;
            break;
        }

        //pause
        if (Input.GetKeyDown(KeyCode.Escape)) {
            paused = !paused;
            pauseMenu.SetActive(!pauseMenu.activeSelf);

            HideTooltip();
        }

        //inventory
        if (Input.GetKeyDown(KeyCode.Tab)) {
            inventory.SetActive(!inventory.activeSelf);

            HideTooltip();
        }

        //block place/destroy
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25) && !inventory.activeSelf && !paused) {
            if (!hit.transform.GetComponent<Block>())
                return;

            selectionGO.SetActive(true);
            selectionGO.transform.position = hit.transform.position;

            if (controlType == ControlType.Pro) {
                if (Input.GetMouseButtonDown(0)) {
                    switch (paintMode) {
                        case PaintMode.Place:
                        GameObject selectedBlock = GameObject.Find("Hotbar").transform.GetChild(selectedSlot).GetComponent<Slot>().item;
                        if (selectedBlock != null)
                            CreateBlock(hit.transform.position + hit.normal, selectedBlock);
                        break;
                        case PaintMode.Break:
                        if (blocks.Count > 1)
                            DestroyBlock(hit.transform.position);
                        break;
                    }

                    transform.position = hit.transform.position;
                }
            } else {
                if (Input.GetMouseButtonDown(0)) {
                    if (blocks.Count > 1)
                        DestroyBlock(hit.transform.position);

                    UpdateBlocksAround(hit.transform.position);
                    transform.position = hit.transform.position;
                } else if (Input.GetMouseButtonDown(1)) {
                    GameObject selectedBlock = GameObject.Find("Hotbar").transform.GetChild(selectedSlot).GetComponent<Slot>().item;
                    if (selectedBlock != null)
                        CreateBlock(hit.transform.position + hit.normal, selectedBlock);

                    transform.position = hit.transform.position;
                }
            }
        } else {
            selectionGO.SetActive(false);
        }
    }

    //runs every gui frame
    private void OnGUI() {
        tooltip.transform.position = new Vector3(Input.mousePosition.x + 50, Input.mousePosition.y);
    }

    //update every block face for optimizations
    public void UpdateAllBlocks() {
        foreach (GameObject item in blocks) {
            item.GetComponent<Block>().UpdateBlock(blocks);
        }
    }

    //update every block face around pos for optimizations
    public void UpdateBlocksAround(Vector3 pos) {
        foreach (GameObject block in GetObjectsInRadius(pos, 2, blocks)) {
            block.GetComponent<Block>().UpdateBlock(blocks);
        }
    }

    //create block at position of type block
    public void CreateBlock(Vector3 position, GameObject block) {
        GameObject newBlock = Instantiate(block) as GameObject;
        newBlock.name = block.name + " " + position;
        newBlock.transform.position = position;
        if (newBlock.GetComponent<Block>().faceTowardsPlayerWhenPlaced) {
            newBlock.transform.localEulerAngles = new Vector3(0, Mathf.Round(Camera.main.transform.localEulerAngles.y / 90) * 90, 0);
        }

        blocks.Add(newBlock);
        PlayBlockAudio(newBlock.transform, PaintMode.Place);
        //foreach (GameObject block in GetObjectsInRadius(blocks, 5)) {
        //block.GetComponent<Block>().UpdateBlock();
        //}
        UpdateBlocksAround(position);
    }

    //destroy blocka t position
    public void DestroyBlock(Vector3 position) {
        List<GameObject> blocksToRemove = new List<GameObject>();
        foreach (GameObject block in blocks) {
            if (block.transform.position == position) {
                Destroy(block);
                blocksToRemove.Add(block);
                PlayBlockAudio(block.transform, PaintMode.Break);

                ParticleSystem newParticle = Instantiate(blockBreakParticle);
                newParticle.transform.position = position;
                newParticle.GetComponent<ParticleSystemRenderer>().material = block.transform.GetChild(5).GetComponent<MeshRenderer>().material;
                newParticle.Play();
            }
        }

        foreach (GameObject block in blocksToRemove) {
            blocks.Remove(block);
        }
        UpdateBlocksAround(position);
    }

    //get objects in radius of pos
    List<GameObject> GetObjectsInRadius(Vector3 pos, float radius, List<GameObject> array) {
        List<GameObject> objects = new List<GameObject>();
        foreach (GameObject obj in array) {
            if (Vector3.Distance(pos, obj.transform.position) < radius)
                objects.Add(obj);
        }

        return objects;
    }

    //change paint mode
    public void SetPaintMode(string mode) {
        switch (mode) {
            case "Add":
                paintMode = PaintMode.Place;
                break;
            case "Subtract":
                paintMode = PaintMode.Break;
                break;
        }
    }

    //change slot
    public void SetSelectedSlot(int slot) {
        selectedSlot = slot;
    }

    //force item into slot
    public void PutItemInSelectedSlot(Slot mySlot) {
        GameObject slot = GameObject.Find("Hotbar").transform.GetChild(GameObject.Find("World").GetComponent<World>().selectedSlot).gameObject;
        slot.GetComponent<Slot>().item = mySlot.item;
        slot.GetComponent<MenuButton>().tooltip = mySlot.GetComponent<MenuButton>().tooltip;
        slot.transform.GetChild(0).GetComponent<RawImage>().texture = mySlot.transform.GetChild(0).GetComponent<RawImage>().texture;
        slot.transform.GetChild(0).gameObject.SetActive(true);
    }

    //play block place/break audio
    private void PlayBlockAudio(Transform target, PaintMode mode) {
        AudioClip[] blockAudio = stoneAudio;
        AudioSource audioSource = GetComponent<AudioSource>();
        switch (target.GetComponent<Block>().blockType) {
            case BlockType.Cloth:
            blockAudio = clothAudio;
            audioSource.pitch = clothPitch;
            break;
            case BlockType.Glass:
            if (mode == PaintMode.Place) {
                blockAudio = glassPlaceAudio;
                audioSource.pitch = glassPlacePitch;
            } else {
                blockAudio = glassBreakAudio;
                audioSource.pitch = glassBreakPitch;
            }
            break;
            case BlockType.Grass:
            blockAudio = grassAudio;
            audioSource.pitch = grassPitch;
            break;
            case BlockType.Gravel:
            blockAudio = gravelAudio;
            audioSource.pitch = gravelPitch;
            break;
            case BlockType.Sand:
            blockAudio = sandAudio;
            audioSource.pitch = sandPitch;
            break;
            case BlockType.Snow:
            blockAudio = snowAudio;
            audioSource.pitch = snowPitch;
            break;
            case BlockType.Stone:
            blockAudio = stoneAudio;
            audioSource.pitch = stonePitch;
            break;
            case BlockType.Metal:
            blockAudio = stoneAudio;
            audioSource.pitch = metalPitch;
            break;
            case BlockType.Wood:
            blockAudio = woodAudio;
            audioSource.pitch = woodPitch;
            break;
        }

        GetComponent<AudioSource>().PlayOneShot(blockAudio[Random.Range(0, blockAudio.Length)]);
    }

    //load blocks into slots
    private IEnumerator LoadSlots() {
        var loadedObjects = Resources.LoadAll("Blocks", typeof(GameObject));
        int i = 0;
        foreach (GameObject go in loadedObjects) {
            GameObject newSlot = Instantiate(inventorySlotGO, inventory.transform);

            var newGO = Instantiate(go);
            newGO.transform.position = new Vector3(1000, 0, 0);
            newSlot.GetComponent<Slot>().item = go;

            RenderTexture rt = new RenderTexture(128, 128, 16, RenderTextureFormat.ARGB32);
            GameObject.Find("RenderCamera").GetComponent<Camera>().targetTexture = rt;
            newSlot.transform.GetChild(0).GetComponent<RawImage>().texture = rt;
            yield return new WaitForEndOfFrame();
            i++;
            if (i < loadedObjects.Length)
                Destroy(newGO);
        }
    }

    //resume game
    public void ResumeGame() {
        paused = false;
        pauseMenu.SetActive(false);
    }

    //quit app
    public void QuitGame() {
        Application.Quit();
    }

    //show tooltips
    public void ShowTooltip(MenuButton button) {
        if (button.tooltip == "")
            return;
        GameObject.Find("Tooltip").GetComponent<Image>().enabled = true;
        GameObject.Find("Tooltip").transform.GetChild(0).GetComponent<Text>().enabled = true;
        GameObject.Find("Tooltip").transform.GetChild(0).GetComponent<Text>().text = button.tooltip;
    }

    //hide tooltips
    public void HideTooltip() {
        GameObject.Find("Tooltip").GetComponent<Image>().enabled = false;
        GameObject.Find("Tooltip").transform.GetChild(0).GetComponent<Text>().enabled = false;
        GameObject.Find("Tooltip").transform.GetChild(0).GetComponent<Text>().text = "";
    }

    //set control style
    public void SetControlStyle(Text label) {
        if (label.text == "Professional") {
            controlType = ControlType.Pro;
            proControlsText.SetActive(true);
            fpsControlsText.SetActive(false);

            topPanel.SetActive(true);
        } else {
            controlType = ControlType.FPS;
            fpsControlsText.SetActive(true);
            proControlsText.SetActive(false);

            topPanel.SetActive(false);
        }
    }

    //change sensitivity
    public void ChangeSensitivity() {
        PlayerPrefs.SetInt("Sensitivity", (int)sensitivitySlider.value);
        sensitivityText.text = sensitivitySlider.value.ToString();
    }
}