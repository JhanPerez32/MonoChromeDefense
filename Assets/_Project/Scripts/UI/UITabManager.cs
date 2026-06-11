using System.Collections.Generic;
using UnityEngine;

public class UITabManager : MonoBehaviour
{
    [SerializeField] private TabEntry[] turretShopTabs;
    [SerializeField] private TabTypeScriptable defaultTab;

    private Dictionary<TabTypeScriptable, TabEntry> _tabLookup;

    private void Awake()
    {
        _tabLookup = new Dictionary<TabTypeScriptable, TabEntry>();

        foreach (var tabEntry in turretShopTabs)
        {
            if (tabEntry.tabType == null || tabEntry.panel == null || tabEntry.button == null) continue;

            _tabLookup[tabEntry.tabType] = tabEntry;
            
            TabTypeScriptable capturedTab = tabEntry.tabType;

            tabEntry.button.onClick.AddListener(() =>
            {
                ChangeTab(capturedTab);
            });
        }
    }

    private void Start()
    {
        if (defaultTab != null)
        {
            ChangeTab(defaultTab);
        }
        else if (turretShopTabs.Length > 0)
        {
            ChangeTab(turretShopTabs[0].tabType);
        }
    }

    private void ChangeTab(TabTypeScriptable selectedTab)
    {
        foreach (var tabEntry in turretShopTabs)
        {
            bool isSelected = tabEntry.tabType == selectedTab;

            tabEntry.panel.SetActive(isSelected);
            tabEntry.button.interactable = !isSelected;
        }
    }
}
