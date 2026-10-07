from odoo import models, fields

class CvmsPosition(models.Model):
    _name = 'cvms.position'
    _description = 'CVMS Position Stats'

    name = fields.Char(string="Position Title", required=True, readonly=True)
    attribute_ids = fields.One2many('cvms.position.attribute', 'position_id', string="Attributes", readonly=True)

class CvmsPositionAttribute(models.Model):
    _name = 'cvms.position.attribute'
    _description = 'CVMS Position Attribute'

    position_id = fields.Many2one('cvms.position', string="Position", ondelete='cascade')
    name = fields.Char(string="Attribute Title", readonly=True)
    attr_type = fields.Selection([('number', 'Number'), ('text', 'Text')], string="Type", readonly=True)
    aggregated_value = fields.Char(string="Aggregated Result", readonly=True)

