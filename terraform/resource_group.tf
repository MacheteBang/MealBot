resource "azurerm_resource_group" "rg" {
  name     = "${var.owner_code}-${var.system}-rg"
  location = "northcentralus"
  tags     = local.default_tags
}
