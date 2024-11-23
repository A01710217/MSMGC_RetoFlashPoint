using UnityEngine;
using System.Collections.Generic;

public class HouseBuilder : MonoBehaviour {
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject wallDoorPrefab;
    public GameObject wallNotDoorPrefab;
    public GameObject firePrefab;
    public GameObject smokePrefab;
    public GameObject poi_baitPrefab;
    public GameObject poiPrefab;

    public void BuildHouse(MazeGraph graph) {
        // Colocar nodos (pisos, fuego, puntos de interés)
        foreach (var node in graph.nodes) {
            // Posición de la celda
            Vector3 position = new Vector3(node.id[1], 0, node.id[0]);

            // Colocar el piso en cada celda y rotarlo 270 grados en el eje X
            Instantiate(floorPrefab, position, Quaternion.Euler(270, 0, 0));

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
                    //Checar el status del poi
                    if (node.status == "v") {
                        // Colocar el punto de interés
                        Instantiate(poiPrefab, position + Vector3.up * 0.1f, Quaternion.identity);
                    } else {
                        // Colocar el punto de interés con cebo
                        Instantiate(poi_baitPrefab, position + Vector3.up * 0.1f, Quaternion.identity);
                    }
                    break;
                default:
                    // Otros tipos no necesitan acción aquí
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
                    Instantiate(wallPrefab, midPoint, rotation);
                    break;
                case "door":
                    Instantiate(wallDoorPrefab, midPoint, rotation);
                    break;
                case "exit":
                    Instantiate(wallNotDoorPrefab, midPoint, rotation);
                    break;
                default:
                    // "empty" o conexiones sin categoría no hacen nada
                    break;
            }
        }
    }

    // Obtener la rotación de la pared o puerta según su orientación
    private Quaternion GetEdgeRotation(Vector3 source, Vector3 target) {
        // Si la diferencia es en Z, es una conexión vertical
        if (Mathf.Abs(source.z - target.z) > 0.1f) {
            return Quaternion.Euler(270, 0, 0); // Sin rotación adicional
        }

        // Si la diferencia es en X, es una conexión horizontal
        if (Mathf.Abs(source.x - target.x) > 0.1f) {
            return Quaternion.Euler(270, 90, 0); // Rotación 90° en el eje Y
        }

        // Por defecto, sin rotación
        return Quaternion.identity;
    }

    // Métodos para crear objetos predeterminados si faltan prefabs
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
