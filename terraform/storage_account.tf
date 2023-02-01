resource "azurerm_storage_account" "st" {
  name                     = lower("${var.owner_code}${var.system}st")
  resource_group_name      = azurerm_resource_group.rg.name
  location                 = azurerm_resource_group.rg.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
  tags                     = local.default_tags
}
