using UnityEngine;
using System.Collections;

public class ArcBall : MonoBehaviour {
	
	Matrix4x4 m_mRotation;         	// Matrix for arc ball's orientation
	Matrix4x4 m_mTranslation;      	// Matrix for arc ball's position
	Matrix4x4 m_mTranslationDelta; 	// Matrix for arc ball's position
	
	int m_nWidth;   			// arc ball's window width
	int m_nHeight;  			// arc ball's window height
	Vector2 m_vCenter;  		// center of arc ball 
	float m_fRadius;  			// arc ball's radius in screen coords
	float m_fRadiusTranslation; // arc ball's radius for translating the target
	
	Quaternion m_qDown;       	// Quaternion before button down
	Quaternion m_qNow;        	// Composite quaternion for current drag
	bool m_bDrag;             	// Whether user is dragging arc ball
	
	Vector2 m_ptLastMouse;		// position of last mouse point
	Vector3 m_vDownPt;			// starting point of rotation arc
	Vector3 m_vCurrentPt;		// current point of rotation arc

	void ResetParam(){

		m_qDown = Quaternion.identity;;
		m_qNow = Quaternion.identity;
		m_mRotation = Matrix4x4.identity ;
		m_mTranslation = Matrix4x4.identity ;
		m_mTranslationDelta = Matrix4x4.identity ;
		m_bDrag = false;
		m_fRadiusTranslation = 1.0f;
		m_fRadius = 1.0f;

	}

	void SetWindow( int nWidth, int nHeight ) {
		
		float fRadius = 0.9f;
		
		m_nWidth = nWidth; 
		m_nHeight = nHeight; 
		m_fRadius = fRadius; 
		m_vCenter = new Vector2(m_nWidth/2.0f,m_nHeight/2.0f); 
	}

	Vector3 ScreenToVector( float fScreenPtX , float fScreenPtY ){
		
		// Scale to screen
		float x = (fScreenPtX - m_nWidth/2)  / (m_fRadius*m_nWidth/2);
		float y =  (fScreenPtY - m_nHeight/2) / (m_fRadius*m_nHeight/2);
		
		float z = 0.0f;
		float mag = x*x + y*y;
		
		if( mag > 1.0 )
		{
			float scale = 1.0f/Mathf.Sqrt(mag);
			x *= scale;
			y *= scale;
		}
		else
			z = Mathf.Sqrt( 1.0f - mag );
		
		// Return vector
		return new Vector3( x, y, z );
		
	}

	Quaternion QuatFromBallPoints( Vector3 vFrom, Vector3 vTo )
	{
		float fDot = Vector3.Dot(vFrom, vTo);
		Vector3 vPart = Vector3.Cross(vFrom, vTo);
		
		return new Quaternion(vPart.x, vPart.y, vPart.z, fDot);
	}
	
	void OnBegin( int nX , int nY )
	{
		m_bDrag = true;
		m_vDownPt = ScreenToVector( nX, nY );
	}
	
	void OnMove( int nX, int nY )
	{
		if (m_bDrag) 
		{ 
			m_vCurrentPt = ScreenToVector( nX, nY );
			m_qNow = m_qDown * QuatFromBallPoints( m_vDownPt, m_vCurrentPt );
		}
	}
	
	void OnEnd()
	{
		m_bDrag = false;
		m_qDown = m_qNow;
	}

	// Use this for initialization
	void Start () {
		ResetParam();
		m_vDownPt = new Vector3(0,0,0);
		m_vCurrentPt = new Vector3(0,0,0);
		
		SetWindow( Screen.width, Screen.height );
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButtonDown(0)){
			Debug.Log("Mouse Down:" + Time.frameCount);
			OnBegin( (int)Input.mousePosition.x, (int)Input.mousePosition.y);
		}
		
		OnMove((int)Input.mousePosition.x, (int)Input.mousePosition.y);
		
		if( Input.GetMouseButtonUp(0)){
			Debug.Log("Mouse Up:" + Time.frameCount);
			OnEnd();
		}
		
		transform.rotation = Quaternion.Inverse(m_qNow); 	

	}
}
