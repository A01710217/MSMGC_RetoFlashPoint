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

    public void BuildHouse(MazeGraph graph, List<AgentState> agents, GameObject agentPrefab) {
        // Colocar nodos (pisos, fuego, puntos de interés)
        foreach (var node in graph.nodes) {
            Vector3 position = new Vector3(node.id[1], 0, node.id[0]);
            Instantiate(floorPrefab, position, Quaternion.Euler(0, 0, 0));

            switch (node.type) {
                case "fire":
                    Instantiate(firePrefab, position + Vector3.up * 0.1f, Quaternion.identity);
                    break;
                case "smoke":
                    Instantiate(smokePrefab, position + Vector3.up * 0.1f, Quaternion.identity);
                    break;
                case "poi":
                    if (node.status == "v") {
                        Instantiate(poiPrefab, position + Vector3.up * 0.1f, Quaternion.identity);
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
            Vector3 midPoint = (sourcePos + targetPos) / 2;
            Quaternion rotation = GetEdgeRotation(sourcePos, targetPos);

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
                    break;
            }
        }

        // Colocar agentes
        foreach (var agent in agents) {
            Vector3 agentPosition = new Vector3(agent.current_node[1], 0f, agent.current_node[0]);
            Instantiate(agentPrefab, agentPosition, Quaternion.identity);
        }
    }

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
