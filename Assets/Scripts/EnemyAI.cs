using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;


public class EnemyAI : MonoBehaviour
{
    //fark etmesi i�in
    public Transform player;
    public float detectionAngle = 60f; // Fark etme a��s� varsay�lan (60 derece)
    public float detectionDistance = 10f; // = chaseDistance          // Fark etme mesafesi varsay�lan (10 birim)
    //Takip etmesi i�in
    public GameObject Target;
    NavMeshAgent _agent;
    //geri ka�mas� i�in
    public float chaseDistance = 9f;
    public float retreatDistance = 7f;
    private bool playerDetected = false;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        EnemyFind();
        draw();

    }
    #region Karakteri fark et  
    //Karakteri fark etmeyi sa�layan metod
    public void EnemyFind()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position); //geri ka�mas� i�in

        Vector3 playerDirection = player.position - transform.position;
        float angle = Vector3.Angle(playerDirection, transform.forward);

        if (angle < detectionAngle * 0.5f ) //a��y� kontrol ediyor
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, playerDirection, out hit, detectionDistance)) //raycast ile �arp��ma olup olmad���n� kontrol ediyor
            {
                if (hit.collider.CompareTag("Player")) 
                {
                    playerDetected = true;
                    
                }
            }
            
        }
        
        if(playerDetected)
        {
            _agent.SetDestination(Target.transform.position);
            Debug.Log("Oyuncu tespit edildi!");
            transform.LookAt(player);
        }


        if (distanceToPlayer <= chaseDistance && (playerDetected)) 
        {
            if (distanceToPlayer >= retreatDistance)
            {
                
                // Oyuncudan uzakla�
                Vector3 direction = transform.position - player.position;
                Vector3 retreatPosition = transform.position + direction.normalized * retreatDistance;

                NavMeshHit hit;
                if (NavMesh.SamplePosition(retreatPosition, out hit, retreatDistance, NavMesh.AllAreas))
                {
                    _agent.SetDestination(hit.position);
                    Debug.Log("Ka�!");
                }
            }
        }
    }
    #endregion

    #region �iz
    public void draw()
    {
        Debug.DrawRay(transform.position, transform.forward * 4, Color.red);
        Debug.DrawLine(transform.position, player.position);
    }
    #endregion
}