resource "azurerm_windows_function_app" "func" {
  name                       = "${var.owner_code}-${var.system}-func"
  resource_group_name        = azurerm_resource_group.rg.name
  location                   = azurerm_resource_group.rg.location
  storage_account_name       = azurerm_storage_account.st.name
  storage_account_access_key = azurerm_storage_account.st.primary_access_key
  service_plan_id            = azurerm_service_plan.plan.id
  tags                       = local.default_tags

  app_settings = {
    KEY_VAULT_URI = azurerm_key_vault.kv.vault_uri
    SMS_SCHEDULE  = "30 19 * * *"
  }

  identity {
    type = "SystemAssigned"
  }

  site_config {}
}
