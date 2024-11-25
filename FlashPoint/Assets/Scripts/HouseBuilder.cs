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

    private Dictionary<Vector3, GameObject> objectsOnMap = new Dictionary<Vector3, GameObject>();
    private Dictionary<Vector3, GameObject> edgesOnMap = new Dictionary<Vector3, GameObject>();

    // Método para construir o actualizar la casa con los datos del grafo
    public void BuildHouse(MazeGraph graph) {
        ClearMap();  // Limpiar el mapa antes de construirlo nuevamente

        foreach (var node in graph.nodes) {
            Vector3 position = new Vector3(node.id[1], 0, node.id[0]);

            // Crear el suelo
            Instantiate(floorPrefab, position, Quaternion.Euler(0, 0, 0));

            // Crear objetos según el tipo de nodo
            switch (node.type) {
                case "fire":
                    var fire = Instantiate(firePrefab, position + Vector3.up * 0.1f, Quaternion.identity);
                    objectsOnMap[position] = fire;  // Guardar el objeto en el mapa
                    break;
                case "smoke":
                    var smoke = Instantiate(smokePrefab, position + Vector3.up * 0.1f, Quaternion.identity);
                    objectsOnMap[position] = smoke;  // Guardar el objeto en el mapa
                    break;
                case "poi":
                    if (node.status == "v") {
                        var poi = Instantiate(poiPrefab, position + Vector3.up * 0.1f, Quaternion.identity);
                        objectsOnMap[position] = poi;  // Guardar el objeto en el mapa
                    } else {
                        var poiBait = Instantiate(poi_baitPrefab, position + Vector3.up * 0.1f, Quaternion.identity);
                        objectsOnMap[position] = poiBait;  // Guardar el objeto en el mapa
                    }
                    break;
                default:
                    break;
            }
        }

        // Colocar las aristas (muros, puertas, salidas)
        foreach (var edge in graph.edges) {
            Vector3 sourcePos = new Vector3(edge.source[1], 0, edge.source[0]);
            Vector3 targetPos = new Vector3(edge.target[1], 0, edge.target[0]);
            Vector3 midPoint = (sourcePos + targetPos) / 2;
            Quaternion rotation = GetEdgeRotation(sourcePos, targetPos);

            GameObject edgeObject = null;

            switch (edge.category) {
                case "wall":
                    edgeObject = Instantiate(wallPrefab, midPoint, rotation);
                    break;
                case "door":
                    edgeObject = Instantiate(wallDoorPrefab, midPoint, rotation);
                    break;
                case "exit":
                    edgeObject = Instantiate(wallNotDoorPrefab, midPoint, rotation);
                    break;
                default:
                    break;
            }

            if (edgeObject != null) {
                edgesOnMap[midPoint] = edgeObject;  // Guardar la arista en el diccionario
            }
        }
    }

    // Función para limpiar el mapa antes de reconstruirlo
    private void ClearMap() {
        foreach (var obj in objectsOnMap.Values) {
            Destroy(obj);  // Eliminar los nodos del mapa
        }
        objectsOnMap.Clear();

        foreach (var edge in edgesOnMap.Values) {
            Destroy(edge);  // Eliminar las aristas del mapa
        }
        edgesOnMap.Clear();
    }

    // Función para actualizar el estado de nodos y aristas en el mapa
    public void UpdateMapWithChanges(MazeGraph updatedGraph) {
        // Actualizar nodos
        foreach (var node in updatedGraph.nodes) {
            Vector3 position = new Vector3(node.id[1], 0, node.id[0]);

            // Si el nodo es un fuego apagado, eliminarlo
            if (node.type == "fire" && node.status == "inactive") {
                RemoveObjectAtPosition(position);
            }

            // Si el nodo es un POI recogido, eliminarlo
            if (node.type == "poi" && node.status == "f") {
                RemoveObjectAtPosition(position);
            }
        }

        // Actualizar las aristas (muros, puertas, salidas)
        foreach (var edge in updatedGraph.edges) {
            Vector3 sourcePos = new Vector3(edge.source[1], 0, edge.source[0]);
            Vector3 targetPos = new Vector3(edge.target[1], 0, edge.target[0]);
            Vector3 midPoint = (sourcePos + targetPos) / 2;

            // Si la arista es un muro o puerta que debe convertirse en vacío
            if (edge.category == "empty" && edgesOnMap.ContainsKey(midPoint)) {
                Destroy(edgesOnMap[midPoint]);  // Eliminar la arista
                edgesOnMap.Remove(midPoint);
                Debug.Log($"Edge at {midPoint} removed.");
            }
        }
    }

    // Método para eliminar un objeto en una posición específica
    private void RemoveObjectAtPosition(Vector3 position) {
        if (objectsOnMap.ContainsKey(position)) {
            Destroy(objectsOnMap[position]);
            objectsOnMap.Remove(position);
            Debug.Log($"Object at {position} removed.");
        }
    }

    // Función para determinar la rotación de los objetos (muros, puertas, etc.)
    private Quaternion GetEdgeRotation(Vector3 source, Vector3 target) {
        if (Mathf.Abs(source.z - target.z) > 0.1f) {
            return Quaternion.Euler(0, 0, 0);
        }

        if (Mathf.Abs(source.x - target.x) > 0.1f) {
            return Quaternion.Euler(0, 90, 0);
        }

        return Quaternion.identity;
    }
}
