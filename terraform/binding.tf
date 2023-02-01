resource "azurerm_app_service_custom_hostname_binding" "hst_bnd" {
  hostname            = join(".", [azurerm_dns_cname_record.dns_cname.name, azurerm_dns_cname_record.dns_cname.zone_name])
  app_service_name    = azurerm_windows_function_app.func.name
  resource_group_name = azurerm_windows_function_app.func.resource_group_name
}

resource "azurerm_app_service_managed_certificate" "mcrt" {
  custom_hostname_binding_id = azurerm_app_service_custom_hostname_binding.hst_bnd.id
  tags                       = local.default_tags
}

resource "azurerm_app_service_certificate_binding" "MealBot_crt_bnd" {
  hostname_binding_id = azurerm_app_service_custom_hostname_binding.hst_bnd.id
  certificate_id      = azurerm_app_service_managed_certificate.mcrt.id
  ssl_state           = "SniEnabled"
}
