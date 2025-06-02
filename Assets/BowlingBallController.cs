using UnityEngine;
using UnityEngine.UI;

public class BowlingBallController : MonoBehaviour
{
    public Camera mainCamera;
    public Slider directionSlider;
    public Transform startPoint;
    public float forwardSpeed = 10f;
    public float horizontalRange = 3f;
    public bool isLaunched = false;
    public bool followSliderWhileMoving = false;
    public float sliderFollowSpeed = 5f;
    public float maxCurveAmount = 0.5f;

    private Vector3 initialDirection;
    private float curveOffset;
    private float curveSpeed;
    private float wobbleStrength;
    private Rigidbody rb;
void Start()
{
    rb = GetComponent<Rigidbody>();
    ResetBall();
}

    void Update()
{
    if (!isLaunched)
    {
        float sliderZ = Mathf.Lerp(-horizontalRange, horizontalRange, directionSlider.value);
        transform.position = new Vector3(startPoint.position.x, startPoint.position.y, sliderZ);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isLaunched = true;
            initialDirection = transform.right * forwardSpeed;
            rb.velocity = initialDirection;
        }
    }
    else
    {
        if (followSliderWhileMoving)
        {
            float sliderZ = Mathf.Lerp(-horizontalRange, horizontalRange, directionSlider.value);
            float wobble = Mathf.Sin(Time.time * 10f) * wobbleStrength;
            float smoothZ = Mathf.Lerp(transform.position.z, sliderZ + wobble, Time.deltaTime * sliderFollowSpeed);
            rb.velocity = new Vector3(initialDirection.x, 0f, (smoothZ - transform.position.z) * 10f); // z-drift
        }
        else
        {
            // Natural wobble (not zigzag)
            float wildZ = Mathf.PerlinNoise(Time.time * 0.5f, 0f) - 0.5f;
            float drift = wildZ * 2f;
            rb.velocity = new Vector3(initialDirection.x, 0f, drift);
        }
    }
}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pin"))
        {
            Debug.Log("Hit pin!");
            mainCamera.backgroundColor = Color.red;
            Invoke(nameof(ResetBackground), 0.1f);

            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 forceDir = (collision.transform.position - transform.position).normalized;
                forceDir.y = 1f;
                rb.AddForce(forceDir * 8f, ForceMode.Impulse);
                rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);
            }
        }
    }

    private void ResetBackground()
    {
        mainCamera.backgroundColor = Color.black;
    }

    public void ResetBall()
    {
        isLaunched = false;
        transform.position = startPoint.position;
    }
}
