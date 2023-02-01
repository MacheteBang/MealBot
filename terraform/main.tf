// Provider Setup

terraform {
  required_providers {
    azurerm = {
      source = "hashicorp/azurerm"
    }
  }

  # Configure an Azure Container for storing state
  backend "azurerm" {
    resource_group_name  = "mmp-rg-shd"
    storage_account_name = "mmpstgshd"
    container_name       = "terraform"
    key                  = "MealBot.tfstate"
    subscription_id      = "bdef1539-c156-4919-b79f-b6042255b853"
  }
}

provider "azurerm" {
  features {}
}


// Variables
variable "owner_code" {
  description = "Three letter code of the owner of the system."
  type        = string
  default     = "mmp"
}

variable "system" {
  description = "Name of the system that is being built."
  type        = string
  default     = "MealBot"
}

// Data
data "azurerm_dns_zone" "dnsz" {
  name                = "mmmpizza.io"
  resource_group_name = "mmp-rg-shd"
}

locals {
  default_tags = {
    "CreatedBy" = "Terraform"
    "App"       = var.system
  }
}

data "azurerm_client_config" "current" {}
