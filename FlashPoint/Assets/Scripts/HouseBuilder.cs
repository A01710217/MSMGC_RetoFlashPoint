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
<<<<<<< HEAD
    public GameObject poiSmokePrefab;
    public GameObject poi_baitSmokePrefab;
=======
>>>>>>> e854601bc8094a9ac0162741347e31214c553b33

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
<<<<<<< HEAD
                    // Validar si hay un poi y eliminarlo si existe
                    if (objectsOnMap.ContainsKey(position)) {
                        Destroy(objectsOnMap[position]);
                    }
=======
>>>>>>> e854601bc8094a9ac0162741347e31214c553b33
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
<<<<<<< HEAD
                case "poi-smoke":
                    if (node.status == "v") {
                        var poi = Instantiate(poiSmokePrefab, position + Vector3.up * 0.1f, Quaternion.identity);
                        objectsOnMap[position] = poi;  // Guardar el objeto en el mapa
                    } else {
                        var poiBait = Instantiate(poi_baitSmokePrefab, position + Vector3.up * 0.1f, Quaternion.identity);
                        objectsOnMap[position] = poiBait;  // Guardar el objeto en el mapa
                    }
                    break;
=======
>>>>>>> e854601bc8094a9ac0162741347e31214c553b33
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
                    // Validar si el estatus de la puerta es abierta o cerrada
                    if (edge.status == "closed") {
                        edgeObject = Instantiate(wallDoorPrefab, midPoint, rotation);
                    }
                    else if (edge.status == "open") {
                        edgeObject = Instantiate(wallNotDoorPrefab, midPoint, rotation);
                    }
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
<<<<<<< HEAD

=======
>>>>>>> e854601bc8094a9ac0162741347e31214c553b33
        foreach (var obj in objectsOnMap.Values) {
            Destroy(obj);  // Eliminar los nodos del mapa
        }
        objectsOnMap.Clear();

        foreach (var edge in edgesOnMap.Values) {
            Destroy(edge);  // Eliminar las aristas del mapa
        }
        edgesOnMap.Clear();
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