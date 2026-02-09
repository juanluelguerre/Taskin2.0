---
name: update-translations
description: Add or update translation keys in en.json and es.json
user_invocable: true
---

# Update Translations Skill

Add or update translation keys in both language files.

## Arguments

The user provides translation keys and their values, or a feature name to add translations for.

## Translation Files

- English: `ui/src/src/assets/i18n/en.json`
- Spanish: `ui/src/src/assets/i18n/es.json`

## Steps

1. **Read both translation files** to understand current structure

2. **Add/update keys** following the existing nested structure:
   ```json
   {
     "feature": {
       "title": "Feature Title",
       "description": "Feature description",
       "actions": {
         "create": "Create Feature",
         "edit": "Edit Feature",
         "delete": "Delete Feature"
       },
       "messages": {
         "created": "Feature created successfully",
         "updated": "Feature updated successfully",
         "deleted": "Feature deleted successfully"
       },
       "errors": {
         "loadFailed": "Failed to load features",
         "createFailed": "Failed to create feature"
       }
     }
   }
   ```

3. **Ensure both files have identical key structure** — every key in `en.json` must exist in `es.json` and vice versa

4. **Run translation key finder** (if available):
   ```bash
   cd ui/src && npx transloco-keys-manager find
   ```

## Key Naming Conventions

- Dot-separated, nested: `feature.section.key`
- Actions: `feature.actions.{verb}` (create, edit, delete, save, cancel)
- Messages: `feature.messages.{past-tense}` (created, updated, deleted)
- Errors: `feature.errors.{action}Failed` (loadFailed, createFailed)
- Labels: `feature.labels.{fieldName}` (name, description, status)
- Common: `common.{key}` for shared keys (save, cancel, delete, loading, noResults)

## Rules

- ALWAYS add to BOTH `en.json` and `es.json`
- Use proper Spanish translations (not machine-translated placeholders)
- Maintain alphabetical ordering within sections
- Keep existing keys unchanged unless explicitly updating
