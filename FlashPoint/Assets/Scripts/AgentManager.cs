using UnityEngine;
using System.Collections.Generic;

public class AgentManager : MonoBehaviour {
    public GameObject agentPrefab;  // Prefab para los agentes
    private List<GameObject> agents = new List<GameObject>();

    // Método para instanciar agentes en la escena
    public void CreateAgents(List<AgentState> initialAgents) {
        foreach (var agent in initialAgents) {
            Vector3 agentPosition = new Vector3(agent.current_node[1], 0f, agent.current_node[0]);
            GameObject newAgent = Instantiate(agentPrefab, agentPosition, Quaternion.identity);
            newAgent.name = $"Agent_{agent.agent_id}";
            agents.Add(newAgent);
        }
    }

    // Método para actualizar los estados de los agentes
    public void UpdateAgentStates(List<AgentState> agentStates) {
        foreach (var state in agentStates) {
            GameObject agent = agents.Find(a => a.name == $"Agent_{state.agent_id}");
            if (agent != null) {
                Vector3 newPosition = new Vector3(state.current_node[1], 0f, state.current_node[0]);
                agent.transform.position = newPosition;  // Actualiza la posición del agente
            }
        }
    }
}
