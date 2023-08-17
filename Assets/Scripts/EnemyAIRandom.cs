using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;


public class EnemyAIRandom : MonoBehaviour
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
    //Rastgele hareket etmesi i�in
    public NavMeshAgent agent;
    public float range; //k�re yar��ap�
    public Transform centrePoint; //d��man�n hareket etmek istedi�i alan�n merkezi
    bool RandomFinish = false;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        EnemyFind();
        draw();
        if(!RandomFinish) 
        {
            RandomMovement();
        }

    }
    #region Karakteri fark et  
    //Karakteri fark etmeyi sa�layan metod
    public void EnemyFind()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position); //geri ka�mas� i�in

        Vector3 playerDirection = player.position - transform.position;
        float angle = Vector3.Angle(playerDirection, transform.forward);

        if (angle < detectionAngle * 0.5f) //a��y� kontrol ediyor
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, playerDirection, out hit, detectionDistance)) //raycast ile �arp��ma olup olmad���n� kontrol ediyor
            {
                if (hit.collider.CompareTag("Player"))
                {
                    playerDetected = true;
                    RandomFinish = true;    

                }
            }

        }

        if (playerDetected)
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

    #region Rastgele harek i�in
    void RandomMovement()
    {
        if (agent.remainingDistance <= agent.stoppingDistance) 
        {
            Vector3 point;
            if (RandomPoint(centrePoint.position, range, out point)) //merkez noktas�ndan ve alan yar��ap�ndan ge�mek i�in
            {
                Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f); //gizmos ile g�rebilmen i�in
                agent.SetDestination(point);
            }
        }
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {

        Vector3 randomPoint = center + Random.insideUnitSphere * range; //merkezden rastgele nokta
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) //d�k�man: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
        {
            //1.0f, rastgele noktadan navmesh �zerindeki bir noktaya olan maksimum mesafedir, aral�k b�y�kse art�rmay� isteyebilir
            
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
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