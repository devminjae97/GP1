using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * 24.03.02.
 * @조민재, 최초 작성
 */

public class InventoryManager : Singleton<InventoryManager>
{
    [SerializeField] private GameObject _drawer = null;

    private bool _opened = false;
 
    //private List<ItemInstance> Grid;
    
    public void ToggleInventory()
        => OpenInventory(!_opened);   
    
    public void OpenInventory(bool open)
    {
        _opened = open;
        _drawer.SetActive(open);
    }

    public void AddItem()
    {
        
    }
    
    public void RemoveItem()
    {
        
    }
    
    public void UseItem()
    {
        
    }
    
    #region #### Unity

    private void Awake()
    {
        if (_drawer == null)
        {
            EditorLog.Error($"[{nameof(InventoryManager)}] {nameof(_drawer)} is missing.");
        }
    }

    #endregion
}
