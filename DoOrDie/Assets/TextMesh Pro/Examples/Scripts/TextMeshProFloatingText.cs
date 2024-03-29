using UnityEngine;
using System.Collections;
using TMPro;



public class TextMeshProFloatingText : MonoBehaviour 
{
    public Font TheFont;
    
    private GameObject m_floatingText;
    private TextMeshPro m_textMeshPro;
    private TextContainer m_textContainer;
    private TextMesh m_textMesh;
    
    private Transform m_transform;
    private Transform m_floatingText_Transform;
    private Transform m_cameraTransform;

    Vector3 lastPOS = Vector3.zero;
    Quaternion lastRotation = Quaternion.identity;

    public int SpawnType;

    //private int m_frame = 0;

    void Awake()
    {
        m_transform = transform;
        m_floatingText = new GameObject(m_transform.name + " floating text");

        m_floatingText_Transform = m_floatingText.transform;
        m_floatingText_Transform.position = m_transform.position + new Vector3(0, 15f, 0);

        m_cameraTransform = Camera.main.transform;

        //m_parentScript = GetComponent<TextMeshSpawner>();

        //Debug.Log(m_parentScript.NumberOfNPC);
    }

    void Start()
    {
        if (SpawnType == 0)
        {
            //Debug.Log("Spawning TextMesh Pro Objects.");
            // TextMesh Pro Implementation
            m_textMeshPro = m_floatingText.AddComponent<TextMeshPro>();
            m_textContainer = m_floatingText.GetComponent<TextContainer>();
            
            m_textContainer.isAutoFitting = false;
            //m_textMeshPro.fontAsset = Resources.Load("Fonts & Materials/JOKERMAN SDF", typeof(TextMeshProFont)) as TextMeshProFont; // User should only provide a string to the resource.
            //m_textMeshPro.fontSharedMaterial = Resources.Load("Fonts & Materials/ARIAL SDF 1", typeof(Material)) as Material;
            //m_textContainer.anchorPosition = TextContainerAnchors.Bottom;
            m_textMeshPro.alignment = TextAlignmentOptions.Center;
            m_textMeshPro.color = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
            m_textMeshPro.fontSize = 24;          
            //m_textMeshPro.enableExtraPadding = true;
            //m_textMeshPro.enableShadows = false;
            m_textMeshPro.text = string.Empty;           

            StartCoroutine(DisplayTextMeshProFloatingText());
        }
        else
        {
            //Debug.Log("Spawning TextMesh Objects.");

            m_textMesh = m_floatingText.AddComponent<TextMesh>();
            m_textMesh.font = Resources.Load("Fonts/ARIAL", typeof(Font)) as Font;        
            m_textMesh.renderer.sharedMaterial = m_textMesh.font.material;
            m_textMesh.color = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
            m_textMesh.anchor = TextAnchor.LowerCenter;
            m_textMesh.fontSize = 24;

            StartCoroutine(DisplayTextMeshFloatingText());
        }
        
    }


    //void Update()
    //{
    //    if (SpawnType == 0)
    //    {
    //        m_textMeshPro.SetText("{0}", m_frame);         
    //    }
    //    else
    //    {
    //        m_textMesh.text = m_frame.ToString();
    //    }
    //    m_frame = (m_frame + 1) % 1000;

    //}


    public IEnumerator DisplayTextMeshProFloatingText()
    {      
        float CountDuration = 2.0f; // How long is the countdown alive.    
        float starting_Count = Random.Range(5f, 20f); // At what number is the counter starting at.
        float current_Count = starting_Count;
        
        Vector3 start_pos = m_floatingText_Transform.position;
        Color32 start_color = m_textMeshPro.color;
        float alpha = 255;
        //int int_counter = 0;
   
               
        float fadeDuration = 3 / starting_Count * CountDuration;

        while (current_Count > 0)
        {                                                                
            current_Count -= (Time.deltaTime / CountDuration) * starting_Count;
            
            if (current_Count <= 3)
            {
                //Debug.Log("Fading Counter ... " + current_Count.ToString("f2"));
                alpha = Mathf.Clamp(alpha - (Time.deltaTime / fadeDuration) * 255, 0, 255);                 
            }

            //int_counter = (int)current_Count;                 
            m_textMeshPro.SetText("{0}", (int)current_Count);             

            m_textMeshPro.color = new Color32(start_color.r, start_color.g, start_color.b, (byte)alpha);
  
            // Move the floating text upward each update
            m_floatingText_Transform.position += new Vector3(0, starting_Count * Time.deltaTime , 0);

            // Align floating text perpendicular to Camera.
            if (!lastPOS.Compare(m_cameraTransform.position, 1000) || !lastRotation.Compare(m_cameraTransform.rotation, 1000))
            {
                lastPOS = m_cameraTransform.position;
                lastRotation = m_cameraTransform.rotation;
                m_floatingText_Transform.rotation = lastRotation;
                Vector3 dir = m_transform.position - lastPOS;
                m_transform.forward = new Vector3(dir.x, 0, dir.z);       
            }
          
            yield return new WaitForEndOfFrame();
        }

        //Debug.Log("Done Counting down.");
            
        yield return new WaitForSeconds(Random.Range(0.1f, 1.0f));
            
        m_floatingText_Transform.position = start_pos;
        
        StartCoroutine(DisplayTextMeshProFloatingText());
    }


    public IEnumerator DisplayTextMeshFloatingText()
    {
        float CountDuration = 2.0f; // How long is the countdown alive.    
        float starting_Count = Random.Range(5f, 20f); // At what number is the counter starting at.
        float current_Count = starting_Count;

        Vector3 start_pos = m_floatingText_Transform.position;
        Color32 start_color = m_textMesh.color;
        float alpha = 255;
        int int_counter = 0;

        float fadeDuration = 3 / starting_Count * CountDuration;

        while (current_Count > 0)
        {
            current_Count -= (Time.deltaTime / CountDuration) * starting_Count;

            if (current_Count <= 3)
            {
                //Debug.Log("Fading Counter ... " + current_Count.ToString("f2"));
                alpha = Mathf.Clamp(alpha - (Time.deltaTime / fadeDuration) * 255, 0, 255);
            }

            int_counter = (int)current_Count;
            m_textMesh.text = int_counter.ToString(); 
            //Debug.Log("Current Count:" + current_Count.ToString("f2"));

            m_textMesh.color = new Color32(start_color.r, start_color.g, start_color.b, (byte)alpha);

            // Move the floating text upward each update
            m_floatingText_Transform.position += new Vector3(0, starting_Count * Time.deltaTime, 0);

            // Align floating text perpendicular to Camera.
            if (!lastPOS.Compare(m_cameraTransform.position, 1000) || !lastRotation.Compare(m_cameraTransform.rotation, 1000))
            {
                lastPOS = m_cameraTransform.position;
                lastRotation = m_cameraTransform.rotation;
                m_floatingText_Transform.rotation = lastRotation;
                Vector3 dir = m_transform.position - lastPOS;
                m_transform.forward = new Vector3(dir.x, 0, dir.z);            
            }



            yield return new WaitForEndOfFrame();
        }

        //Debug.Log("Done Counting down.");

        yield return new WaitForSeconds(Random.Range(0.1f, 1.0f));

        m_floatingText_Transform.position = start_pos;

        StartCoroutine(DisplayTextMeshFloatingText());
    }
}
