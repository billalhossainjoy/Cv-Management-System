import requests
from odoo import models, fields
from odoo.exceptions import UserError

class CvmsImportWizard(models.TransientModel):
    _name = 'cvms.import.wizard'
    _description = 'Import CVMS Data Wizard'

    api_token = fields.Char(string="API Token", required=True)

    def action_import_data(self):
        base_url = self.env['ir.config_parameter'].sudo().get_param('cvms.api_url', 'http://localhost:5000')
        api_url = f"{base_url}/api/odoo-integration/position-stats?token={self.api_token}"
        
        try:
            response = requests.get(api_url)
            response.raise_for_status()
            data = response.json()
        except Exception as e:
            raise UserError(f"Failed to fetch data from CVMS. Error: {str(e)}")

        position_vals = {
            'name': data.get('positionTitle'),
            'attribute_ids': []
        }

        for attr in data.get('attributes', []):
            position_vals['attribute_ids'].append((0, 0, {
                'name': attr.get('title'),
                'attr_type': attr.get('type'),
                'aggregated_value': attr.get('aggregatedValue'),
            }))

        self.env['cvms.position'].create(position_vals)
        
        return {'type': 'ir.actions.client', 'tag': 'reload'}

