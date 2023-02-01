resource "azurerm_service_plan" "plan" {
  name                = "${var.owner_code}-${var.system}-plan"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  os_type             = "Windows"
  sku_name            = "Y1"
  tags                = local.default_tags
}
