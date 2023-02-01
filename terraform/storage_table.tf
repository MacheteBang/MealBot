resource "azurerm_storage_table" "st_table_family_members" {
  name                 = "FamilyMembers"
  storage_account_name = azurerm_storage_account.st.name
}

resource "azurerm_storage_table" "st_table_meals" {
  name                 = "Meals"
  storage_account_name = azurerm_storage_account.st.name
}

resource "azurerm_storage_table" "st_table_sms_messages" {
  name                 = "SmsMessages"
  storage_account_name = azurerm_storage_account.st.name
}
