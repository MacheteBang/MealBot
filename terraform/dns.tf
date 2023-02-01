resource "azurerm_dns_txt_record" "dns_txt" {
  name                = lower("asuid.${var.system}.mmmpizza.io")
  zone_name           = data.azurerm_dns_zone.dnsz.name
  resource_group_name = data.azurerm_dns_zone.dnsz.resource_group_name
  ttl                 = 300
  tags                = local.default_tags

  record {
    value = azurerm_windows_function_app.func.custom_domain_verification_id
  }
}

resource "azurerm_dns_cname_record" "dns_cname" {
  name                = lower(var.system)
  zone_name           = data.azurerm_dns_zone.dnsz.name
  resource_group_name = data.azurerm_dns_zone.dnsz.resource_group_name
  ttl                 = 300
  record              = azurerm_windows_function_app.func.default_hostname
  tags                = local.default_tags
}
