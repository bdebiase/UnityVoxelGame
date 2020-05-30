using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {
    //INSTANCE VARIABLES
    public GameObject[] frontFaces, backFaces, leftFaces, rightFaces, topFaces, bottomFaces;
    public BlockType blockType;
    public bool keepsOtherFaces;
    public bool faceTowardsPlayerWhenPlaced;

    private World world;

    //run at start of program
    private void Start() {
        world = FindObjectOfType<World>();

        //update block info
        BlockData blockData = GetComponent<BlockData>();
        blockData.xPos = transform.position.x;
        blockData.yPos = transform.position.y;
        blockData.zPos = transform.position.z;
        blockData.yRot = transform.localEulerAngles.y;

        Debug.Log("(" + blockData.xPos + ", " + blockData.yPos + ", " + blockData.zPos + ") Rotation: " + blockData.yRot);
    }

    //update block faces
    public void UpdateBlock(List<GameObject> blocks) {
        bool foundFrontFace = false;
        bool foundBackFace = false;
        bool foundRightFace = false;
        bool foundLeftFace = false;
        bool foundTopFace = false;
        bool foundBottomFace = false;
        foreach (GameObject block in blocks) {
            if (block == null)
                return;
            
            // Front
            if (block.transform.position == transform.position + transform.forward ) {
                if (block.GetComponent<Block>().keepsOtherFaces && !GetComponent<Block>().keepsOtherFaces)
                    continue;

                foreach (GameObject face in frontFaces) {
                    face.GetComponent<MeshRenderer>().enabled = false;
                    foundFrontFace = true;
                }
            } else {
                if (!foundFrontFace) {
                    foreach (GameObject face in frontFaces) {
                        face.GetComponent<MeshRenderer>().enabled = true;
                    }
                }
            }

            // Back
            if (block.transform.position == transform.position - transform.forward) {
                if (block.GetComponent<Block>().keepsOtherFaces && !GetComponent<Block>().keepsOtherFaces)
                    continue;

                foreach (GameObject face in backFaces) {
                    face.GetComponent<MeshRenderer>().enabled = false;
                    foundBackFace = true;
                }
            } else {
                if (!foundBackFace) {
                    foreach (GameObject face in backFaces) {
                        face.GetComponent<MeshRenderer>().enabled = true;
                    }
                }
            }

            // Right
            if (block.transform.position == transform.position + transform.right) {
                if (block.GetComponent<Block>().keepsOtherFaces && !GetComponent<Block>().keepsOtherFaces)
                    continue;

                foreach (GameObject face in rightFaces) {
                    face.GetComponent<MeshRenderer>().enabled = false;
                    foundRightFace = true;
                }
            } else {
                if (!foundRightFace) {
                    foreach (GameObject face in rightFaces) {
                        face.GetComponent<MeshRenderer>().enabled = true;
                    }
                }
            }

            // Left
            if (block.transform.position == transform.position - transform.right) {
                if (block.GetComponent<Block>().keepsOtherFaces && !GetComponent<Block>().keepsOtherFaces)
                    continue;

                foreach (GameObject face in leftFaces) {
                    face.GetComponent<MeshRenderer>().enabled = false;
                    foundLeftFace = true;
                }
            } else {
                if (!foundLeftFace) {
                    foreach (GameObject face in leftFaces) {
                        face.GetComponent<MeshRenderer>().enabled = true;
                    }
                }
            }

            // Top
            if (block.transform.position == transform.position + transform.up) {
                if (block.GetComponent<Block>().keepsOtherFaces && !GetComponent<Block>().keepsOtherFaces)
                    continue;

                foreach (GameObject face in topFaces) {
                    face.GetComponent<MeshRenderer>().enabled = false;
                    foundTopFace = true;
                }
            } else {
                if (!foundTopFace) {
                    foreach (GameObject face in topFaces) {
                        face.GetComponent<MeshRenderer>().enabled = true;
                    }
                }
            }

            // Bottom
            if (block.transform.position == transform.position - transform.up) {
                if (block.GetComponent<Block>().keepsOtherFaces && !GetComponent<Block>().keepsOtherFaces)
                    continue;

                foreach (GameObject face in bottomFaces) {
                    face.GetComponent<MeshRenderer>().enabled = false;
                    foundBottomFace = true;
                }
            } else {
                if (!foundBottomFace) {
                    foreach (GameObject face in bottomFaces) {
                        face.GetComponent<MeshRenderer>().enabled = true;
                    }
                }
            }
        }
    }

    //check if matching other gameobject
    private bool IsMatching(GameObject a, GameObject[] b) {
        foreach (GameObject item in b) {
            return a == item;
        }
        return false;
    }
}
