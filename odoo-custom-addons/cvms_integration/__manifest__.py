{
    'name': 'CVMS Integration',
    'version': '1.0',
    'summary': 'Import Position stats from CVMS',
    'depends': ['base'],
    'data': [
        'security/ir.model.access.csv',
        'wizard/import_wizard_views.xml',
        'views/position_views.xml',
    ],
    'installable': True,
    'application': True,
}

