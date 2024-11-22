using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Node {
    public int[] id;
    public string type;
    public int value;
    public string status;
}

[System.Serializable]
public class Edge {
    public int[] source;
    public int[] target;
    public int weight;
    public string category;
    public int life;
}

[System.Serializable]
public class MazeGraph {
    public List<Node> nodes;
    public List<Edge> edges;
    public List<List<string>> maze;
}

public class UnityClient : MonoBehaviour {
    public HouseBuilder houseBuilder;  // Referencia al script HouseBuilder

    void Start() {
        StartCoroutine(DownloadGraph());
    }

    IEnumerator DownloadGraph() {
        // Realizar solicitud POST al servidor
        UnityWebRequest request = UnityWebRequest.PostWwwForm("http://localhost:8585", "");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success) {
            Debug.Log("Response: " + request.downloadHandler.text);

            // Parsear el JSON recibido
            MazeGraph graph = JsonUtility.FromJson<MazeGraph>(request.downloadHandler.text);
            Debug.Log("Nodos recibidos: " + graph.nodes.Count);
            Debug.Log("Aristas recibidas: " + graph.edges.Count);

            // Llamar a BuildHouse para construir la casa con el grafo recibido
            houseBuilder.BuildHouse(graph);  // Llamar al método BuildHouse del HouseBuilder
        } else {
            Debug.Log("Error: " + request.error);
        }
    }
}
