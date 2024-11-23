using UnityEngine;
using System.Collections.Generic;

public class HouseBuilder : MonoBehaviour {
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject breakwallPrefab;
    public GameObject wallDoorPrefab;
    public GameObject wallNotDoorPrefab;
    public GameObject firePrefab;
    public GameObject smokePrefab;
    public GameObject poi_baitPrefab;
    public GameObject poiBPrefab;
    public GameObject poiGPrefab;
    public GameObject poiRPrefab;


    public void BuildHouse(MazeGraph graph) {
        // Colocar nodos (pisos, fuego, puntos de interés)
        foreach (var node in graph.nodes) {
            // Posición de la celda
            Vector3 position = new Vector3(node.id[1], 0, node.id[0]);
            
            // Ajustar el piso más cerca del suelo
            Instantiate(floorPrefab, position + Vector3.down * 0.25f, Quaternion.Euler(270, 0, 0));

            // Determinar el tipo de nodo
            switch (node.type) {
                case "fire":
                    // Colocar el fuego
                    Instantiate(firePrefab, position + Vector3.up * 0.1f, Quaternion.identity);
                    break;
                case "smoke":
                    // Colocar el humo
                    Instantiate(smokePrefab, position + Vector3.up * 0.1f, Quaternion.identity);
                    break;
                case "poi":
                    // Checar el status del poi
                    if (node.status == "v") {
                        //Crear un punto de interés random
                        int random = Random.Range(0, 3);

                        if (random == 0) {
                            Instantiate(poiBPrefab, position + Vector3.up * 0.1f, Quaternion.identity);
                        } else if (random == 1) {
                            Instantiate(poiGPrefab, position + Vector3.up * 0.1f, Quaternion.identity);
                        } else {
                            Instantiate(poiRPrefab, position + Vector3.up * 0.1f, Quaternion.identity);
                        }
                    } else {
                        Instantiate(poi_baitPrefab, position + Vector3.up * 0.1f, Quaternion.identity);
                    }
                    break;
                default:
                    break;
            }
        }

        // Colocar aristas (muros, puertas, salidas)
        foreach (var edge in graph.edges) {
            Vector3 sourcePos = new Vector3(edge.source[1], 0, edge.source[0]);
            Vector3 targetPos = new Vector3(edge.target[1], 0, edge.target[0]);

            // Calcular la posición intermedia para colocar el objeto
            Vector3 midPoint = (sourcePos + targetPos) / 2;

            // Determinar orientación (horizontal o vertical)
            Quaternion rotation = GetEdgeRotation(sourcePos, targetPos);

            // Colocar el objeto según la categoría
            switch (edge.category) {
                case "wall":
                    //Validar si la pared tiene vida
                    if (edge.life == 2) {
                        Instantiate(wallPrefab, midPoint, rotation);
                    } if (edge.life == 1) {
                        Instantiate(breakwallPrefab, midPoint, rotation);
                    }
                    break;
                case "door":
                    Instantiate(wallDoorPrefab, midPoint, rotation);
                    break;
                case "exit":
                    Instantiate(wallNotDoorPrefab, midPoint, rotation);
                    break;
                default:
                    break;
            }
        }
    }

    // Obtener la rotación de la pared o puerta según su orientación
    private Quaternion GetEdgeRotation(Vector3 source, Vector3 target) {
        if (Mathf.Abs(source.z - target.z) > 0.1f) {
            return Quaternion.Euler(270, 0, 0);
        }
        if (Mathf.Abs(source.x - target.x) > 0.1f) {
            return Quaternion.Euler(270, 90, 0);
        }
        return Quaternion.identity;
    }

    private GameObject CreateDefaultFire() {
        GameObject fire = GameObject.CreatePrimitive(PrimitiveType.Cube);
        fire.GetComponent<Renderer>().material.color = Color.red;
        return fire;
    }

    private GameObject CreateDefaultPoi() {
        GameObject poi = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        poi.GetComponent<Renderer>().material.color = Color.blue;
        return poi;
    }
}
