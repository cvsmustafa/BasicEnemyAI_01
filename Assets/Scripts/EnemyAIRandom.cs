using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;


public class EnemyAIRandom : MonoBehaviour
{
    //fark etmesi için
    public Transform player;
    public float detectionAngle = 60f; // Fark etme açýsý varsayýlan (60 derece)
    public float detectionDistance = 10f; // = chaseDistance          // Fark etme mesafesi varsayýlan (10 birim)
    //Takip etmesi için
    public GameObject Target;
    NavMeshAgent _agent;
    //geri kaçmasý için
    public float chaseDistance = 9f;
    public float retreatDistance = 7f;
    private bool playerDetected = false;
    //Rastgele hareket etmesi için
    public NavMeshAgent agent;
    public float range; //küre yarýçapý
    public Transform centrePoint; //düþmanýn hareket etmek istediði alanýn merkezi
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
    //Karakteri fark etmeyi saðlayan metod
    public void EnemyFind()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position); //geri kaçmasý için

        Vector3 playerDirection = player.position - transform.position;
        float angle = Vector3.Angle(playerDirection, transform.forward);

        if (angle < detectionAngle * 0.5f) //açýyý kontrol ediyor
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, playerDirection, out hit, detectionDistance)) //raycast ile çarpýþma olup olmadýðýný kontrol ediyor
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

                // Oyuncudan uzaklaþ
                Vector3 direction = transform.position - player.position;
                Vector3 retreatPosition = transform.position + direction.normalized * retreatDistance;

                NavMeshHit hit;
                if (NavMesh.SamplePosition(retreatPosition, out hit, retreatDistance, NavMesh.AllAreas))
                {
                    _agent.SetDestination(hit.position);
                    Debug.Log("Kaç!");
                }
            }
        }
    }
    #endregion

    #region Rastgele harek için
    void RandomMovement()
    {
        if (agent.remainingDistance <= agent.stoppingDistance) 
        {
            Vector3 point;
            if (RandomPoint(centrePoint.position, range, out point)) //merkez noktasýndan ve alan yarýçapýndan geçmek için
            {
                Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f); //gizmos ile görebilmen için
                agent.SetDestination(point);
            }
        }
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {

        Vector3 randomPoint = center + Random.insideUnitSphere * range; //merkezden rastgele nokta
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) //döküman: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
        {
            //1.0f, rastgele noktadan navmesh üzerindeki bir noktaya olan maksimum mesafedir, aralýk büyükse artýrmayý isteyebilir
            
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }
    #endregion

    #region Çiz
    public void draw()
    {
        Debug.DrawRay(transform.position, transform.forward * 4, Color.red);
        Debug.DrawLine(transform.position, player.position);
    }
    #endregion
}