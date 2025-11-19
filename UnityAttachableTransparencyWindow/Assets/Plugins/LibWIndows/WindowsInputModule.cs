
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class WindowsInputModule : BaseInputModule
{
	public int SubWindowIndex
	{
		get;
		set;
	} = -1;
	
	protected override void Awake()
	{
		base.Awake();
		OnEnable();
		OnSubWindowEvent += OnSubWindowEventDelegate;
	}
	protected override void OnDestroy()
	{
		OnSubWindowEvent -= OnSubWindowEventDelegate;
		base.OnDestroy();
	}
	public override void Process()
	{
	}
	public void Process( GraphicRaycaster raycaster)
	{
		var pointerEventData = new PointerEventData( eventSystem)
        {
            position = m_MousePosition
        };
		var results = new List<RaycastResult>();
		raycaster.Raycast( pointerEventData, results);
		
		GameObject hit = results.Count > 0 ? results[ 0].gameObject : null;
		
		Debug.LogError( results.Count);
		
		// eventSystem.RaycastAll( pointerEventData, m_RaycastResultCache);
        // RaycastResult raycast = FindFirstRaycast( m_RaycastResultCache);
        // pointerEventData.pointerCurrentRaycast = raycast;
		
		// Debug.LogError( SubWindowIndex + ", " + gameObject.name + ", " + m_MousePosition + ", " + raycast);
		
		if( (m_MouseClickFlags & 0x01) != 0)
        {
			pointerEventData.pointerPressRaycast = results.Count > 0 ? results[0] : new RaycastResult();
			ExecuteEvents.Execute( hit, pointerEventData, ExecuteEvents.pointerDownHandler);
			pointerEventData.pointerPress = hit;
            m_MouseClickFlags &= ~0x01;
        }
		if( (m_MouseClickFlags & 0x02) != 0)
        {
            ExecuteEvents.Execute( hit, pointerEventData, ExecuteEvents.pointerUpHandler);
			
			if (pointerEventData.pointerPress == hit)
			{
				ExecuteEvents.Execute(hit, pointerEventData, ExecuteEvents.pointerClickHandler);
			}
			pointerEventData.pointerPress = null;
            m_MouseClickFlags &= ~0x02;
        }
		ExecuteEvents.Execute(hit, pointerEventData, ExecuteEvents.pointerMoveHandler);
	}
	void OnSubWindowEventDelegate( LibWindows.SubWindowEvent ev)
	{
		if( ev.index == SubWindowIndex)
		{
			switch( ev.msg)
			{
				case 0x0200: /* WM_MOUSEMOVE */
				{
					m_MousePosition = new Vector2( ev.x, 512 - ev.y);
					break;
				}
				case 0x0201: /* WM_LBUTTONDOWN */
				{
					m_MouseClickFlags |= 0x01;
					break;
				}
				case 0x0202: /* WM_LBUTTONUP */
				{
					m_MouseClickFlags |= 0x02;
					break;
				}
			}
		}
	}
	[AOT.MonoPInvokeCallback( typeof( LibWindows.SubWindowEventCallback))]
	public static void OnSubWindowEventCallback( LibWindows.SubWindowEvent msg)
	{
		OnSubWindowEvent?.Invoke( msg);
	}
	static event System.Action<LibWindows.SubWindowEvent> OnSubWindowEvent;
	
	[System.NonSerialized]
	Vector2 m_MousePosition;
	[System.NonSerialized]
	int m_MouseClickFlags;
}
