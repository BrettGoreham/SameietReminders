# Configure the Azure provider
terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.26.0"
    }
  }

  required_version = ">= 1.1.0"
}

provider "azurerm" {
  features {}
}

resource "azurerm_resource_group" "sam" {
  name     = "rg-sameiet-reminders"
  location = "norwayeast"
}

resource "azurerm_storage_account" "sam" {
  name                     = "sameietreminderssa"
  resource_group_name      = azurerm_resource_group.sam.name
  location                 = azurerm_resource_group.sam.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

resource "azurerm_service_plan" "sam" {
  name                = "sameiet-reminders-app-service-plan"
  resource_group_name = azurerm_resource_group.sam.name
  location            = azurerm_resource_group.sam.location
  os_type             = "Windows"
  sku_name            = "Y1"
}

resource "azurerm_application_insights" "sam" {
  name                = "sameiet-reminders-insights"
  resource_group_name = azurerm_resource_group.sam.name
  location            = azurerm_resource_group.sam.location
  application_type    = "web"
}

resource "azurerm_windows_function_app" "sam" {
  name                = "sameiet-reminders-function-app"
  resource_group_name = azurerm_resource_group.sam.name
  location            = azurerm_resource_group.sam.location

  storage_account_name       = azurerm_storage_account.sam.name
  storage_account_access_key = azurerm_storage_account.sam.primary_access_key
  service_plan_id            = azurerm_service_plan.sam.id

  site_config {}

  app_settings = merge(
    {"APPINSIGHTS_INSTRUMENTATIONKEY" = azurerm_application_insights.sam.instrumentation_key},
    var.functionAppSettingsMap)
}
