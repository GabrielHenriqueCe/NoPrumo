-- =============================================================================
-- NoPrumo · dados de demonstração
-- =============================================================================
-- Monta uma construtora inteira para testar e apresentar: setores, funções,
-- funcionários, clientes, fornecedores, estoque, treinamentos, equipes, quatro
-- obras com etapas, e cerca de quatro meses de ponto e de entrada de material
-- (o que dá forma aos gráficos do financeiro).
--
-- Antes de rodar:
--   1. banco criado e migrations aplicadas (dotnet ef database update);
--   2. a API iniciada pelo menos uma vez: ela cria permissões, perfis, o admin,
--      as categorias de estoque e os regimes de contratação com os encargos.
--
-- É seguro rodar de novo:
--   - cadastro (setor, funcionário, obra...) só entra se ainda não existir,
--     procurado pelo nome ou pelo código; nada é apagado nem sobrescrito;
--   - ponto, movimentação e aditivo marcados como 'demo' são apagados e gerados
--     de novo, com as datas contadas a partir de hoje.
--
-- Não cria usuários: o admin cria pela tela Users, e liga o mestre ao
-- funcionário dele (Valdir Kühn ou Osni Bortolini) para ver só as obras dele.
-- =============================================================================

SET NAMES utf8mb4;

SET @today       = CURDATE();
SET @yesterday   = @today - INTERVAL 1 DAY;
SET @first_day   = @today - INTERVAL 320 DAY;   -- antes do início da obra mais antiga

-- Se aparecer 0 aqui, a API ainda não subiu nenhuma vez: pare e suba a API antes.
SELECT COUNT(*) AS regimes_cadastrados FROM employment_regime WHERE active = 1;


-- -----------------------------------------------------------------------------
-- 1. Setores e funções
-- -----------------------------------------------------------------------------
INSERT INTO department (name, active)
SELECT source.name, 1
FROM (SELECT 'Engenharia' AS name UNION ALL SELECT 'Obras' UNION ALL SELECT 'Segurança do Trabalho'
      UNION ALL SELECT 'Suprimentos' UNION ALL SELECT 'Almoxarifado' UNION ALL SELECT 'Administrativo'
      UNION ALL SELECT 'Financeiro' UNION ALL SELECT 'Recursos Humanos') AS source
WHERE NOT EXISTS (SELECT 1 FROM department WHERE department.name = source.name);

DROP TEMPORARY TABLE IF EXISTS seed_job_role;
CREATE TEMPORARY TABLE seed_job_role (name VARCHAR(120), department VARCHAR(120));
INSERT INTO seed_job_role VALUES
    ('Engenheiro civil', 'Engenharia'),
    ('Engenheiro eletricista', 'Engenharia'),
    ('Técnico de edificações', 'Engenharia'),
    ('Mestre de obras', 'Obras'),
    ('Encarregado', 'Obras'),
    ('Pedreiro', 'Obras'),
    ('Armador', 'Obras'),
    ('Carpinteiro', 'Obras'),
    ('Eletricista', 'Obras'),
    ('Servente', 'Obras'),
    ('Técnico de segurança do trabalho', 'Segurança do Trabalho'),
    ('Comprador', 'Suprimentos'),
    ('Almoxarife', 'Almoxarifado'),
    ('Assistente administrativo', 'Administrativo'),
    ('Analista financeiro', 'Financeiro'),
    ('Analista de RH', 'Recursos Humanos');

INSERT INTO job_role (name, department_id, active)
SELECT source.name, department.id, 1
FROM seed_job_role AS source
JOIN department ON department.name = source.department
WHERE NOT EXISTS (SELECT 1 FROM job_role WHERE job_role.name = source.name AND job_role.department_id = department.id);


-- -----------------------------------------------------------------------------
-- 2. Funcionários
--    O regime é escolhido pela unidade do salário, não pelo nome, porque o nome
--    muda de um banco para outro: month = salário do mês, hour = valor da hora,
--    day = valor do dia.
-- -----------------------------------------------------------------------------
DROP TEMPORARY TABLE IF EXISTS seed_employee;
CREATE TEMPORARY TABLE seed_employee (
    registration VARCHAR(20), name VARCHAR(160), job_role VARCHAR(120), pay_unit VARCHAR(10),
    pay_rate DECIMAL(12,2), additional DECIMAL(5,2), hired_days_ago INT, phone VARCHAR(20));
INSERT INTO seed_employee VALUES
    ('MAT-1001', 'Valdir Kühn',              'Mestre de obras',                  'month', 6200.00,  0, 2400, '47999010001'),
    ('MAT-1002', 'Osni Bortolini',           'Mestre de obras',                  'month', 5900.00,  0, 1900, '47999010002'),
    ('MAT-1003', 'Jorge Hoffmann',           'Encarregado',                      'month', 3850.00,  0, 1500, '47999010003'),
    ('MAT-1004', 'Luiz Fachini',             'Pedreiro',                         'month', 3420.00,  0, 1300, '47999010004'),
    ('MAT-1005', 'Rafael Moser',             'Pedreiro',                         'hour',      24.50,  0,  700, '47999010005'),
    ('MAT-1006', 'Ezequiel Santos',          'Servente',                         'day',   160.00,  0,  300, '47999010006'),
    ('MAT-1007', 'Adilson Pereira',          'Armador',                          'month', 3600.00,  0, 1100, '47999010007'),
    ('MAT-1008', 'Gilmar Schmitt',           'Carpinteiro',                      'month', 3500.00,  0,  900, '47999010008'),
    ('MAT-1009', 'Cleiton Wolff',            'Eletricista',                      'month', 4100.00, 30,  800, '47999010009'),
    ('MAT-1010', 'Juliano Kreuch',           'Servente',                         'hour',      17.80,  0,  400, '47999010010'),
    ('MAT-1011', 'Rodrigo Hasse',            'Pedreiro',                         'day',   220.00,  0,  250, '47999010011'),
    ('MAT-1012', 'Márcio Zimmermann',        'Servente',                         'month', 2300.00,  0,  600, '47999010012'),
    ('MAT-1013', 'Edson Ramos',              'Armador',                          'hour',      22.00,  0,  500, '47999010013'),
    ('MAT-1014', 'Marta Vieira',             'Técnico de segurança do trabalho', 'month', 5100.00,  0, 1400, '47999010014'),
    ('MAT-1015', 'Patrícia Koch',            'Engenheiro civil',                 'month', 13200.00, 0, 2000, '47999010015'),
    ('MAT-1016', 'Fernando Lenzi',           'Engenheiro eletricista',           'month', 11800.00, 0, 1600, '47999010016'),
    ('MAT-1017', 'Bruna Tomio',              'Técnico de edificações',           'month', 4600.00,  0,  700, '47999010017'),
    ('MAT-1018', 'Débora Müller',            'Comprador',                        'month', 6100.00,  0, 1200, '47999010018'),
    ('MAT-1019', 'Ivo Rengel',               'Almoxarife',                       'month', 3300.00,  0, 1000, '47999010019'),
    ('MAT-1020', 'Simone Hering',            'Assistente administrativo',        'month', 3100.00,  0,  800, '47999010020'),
    ('MAT-1021', 'Carla Buss',               'Analista financeiro',              'month', 7200.00,  0, 1500, '47999010021'),
    ('MAT-1022', 'Tatiane Reis',             'Analista de RH',                   'month', 5800.00,  0, 1100, '47999010022'),
    ('MAT-1023', 'Anderson Moretti',         'Eletricista',                      'hour',      28.00, 30,  350, '47999010023'),
    ('MAT-1024', 'Sérgio Weiss',             'Pedreiro',                         'month', 3420.00,  0, 1700, '47999010024');

INSERT INTO employee (registration_number, name, job_role_id, employment_regime_id, pay_rate,
                      additional_percentage, hire_date, phone, active)
SELECT source.registration, source.name, job_role.id, regime.id, source.pay_rate,
       source.additional, @today - INTERVAL source.hired_days_ago DAY, source.phone, 1
FROM seed_employee AS source
-- A mesma função pode existir em dois setores; uma só por funcionário.
JOIN job_role ON job_role.id = (SELECT MIN(candidate.id) FROM job_role AS candidate WHERE candidate.name = source.job_role)
JOIN employment_regime AS regime
     ON regime.id = (SELECT MIN(candidate.id) FROM employment_regime AS candidate
                     WHERE candidate.unit = source.pay_unit AND candidate.active = 1)
WHERE NOT EXISTS (SELECT 1 FROM employee
                  WHERE employee.registration_number = source.registration OR employee.name = source.name);

-- Daqui para baixo o funcionário é achado pela matrícula do seed ou, se ele já
-- existia com outra matrícula, pelo nome.
DROP TEMPORARY TABLE IF EXISTS seed_employee_id;
CREATE TEMPORARY TABLE seed_employee_id (registration VARCHAR(20) PRIMARY KEY, id BIGINT);
INSERT INTO seed_employee_id
SELECT source.registration,
       (SELECT MIN(employee.id) FROM employee
        WHERE employee.registration_number = source.registration OR employee.name = source.name)
FROM seed_employee AS source;


-- -----------------------------------------------------------------------------
-- 3. Clientes e fornecedores (sem documento: CPF/CNPJ só pela tela, que cifra)
-- -----------------------------------------------------------------------------
INSERT INTO client (name, person_type, email, contact_name, phone, city, state)
SELECT source.name, source.person_type, source.email, source.contact, source.phone, source.city, 'SC'
FROM (SELECT 'Aurora Incorporadora Ltda' AS name, 'company' AS person_type, 'obras@aurora.com.br' AS email,
             'Renata Spengler' AS contact, '4733220001' AS phone, 'Blumenau' AS city
      UNION ALL SELECT 'Itoupava Logística S.A.', 'company', 'engenharia@itoupavalog.com.br', 'Marcos Haas', '4733220002', 'Blumenau'
      UNION ALL SELECT 'Prefeitura de Blumenau', 'company', 'obras@blumenau.sc.gov.br', 'Secretaria de Obras', '4733810000', 'Blumenau'
      UNION ALL SELECT 'Ponta Aguda Empreendimentos', 'company', 'contato@pontaaguda.com.br', 'Eduardo Klug', '4733220004', 'Blumenau') AS source
WHERE NOT EXISTS (SELECT 1 FROM client WHERE client.name = source.name);

INSERT INTO supplier (name, contact_name, phone, email, city, state, active)
SELECT source.name, source.contact, source.phone, source.email, source.city, 'SC', 1
FROM (SELECT 'Cimentos Vale do Itajaí Ltda' AS name, 'Paulo' AS contact, '4733301001' AS phone,
             'vendas@cimentosvale.com.br' AS email, 'Gaspar' AS city
      UNION ALL SELECT 'Aço Forte Distribuidora Ltda', 'Cristiane', '4733301002', 'comercial@acoforte.com.br', 'Blumenau'
      UNION ALL SELECT 'Areal Itajaí-Açu Ltda', 'Nelson', '4733301003', 'pedidos@arealitajai.com.br', 'Indaial'
      UNION ALL SELECT 'Cerâmica Rio do Sul', 'Vanessa', '4735201004', 'vendas@ceramicariodosul.com.br', 'Rio do Sul'
      UNION ALL SELECT 'EPI Sul Equipamentos', 'Rogério', '4733301005', 'loja@episul.com.br', 'Blumenau') AS source
WHERE NOT EXISTS (SELECT 1 FROM supplier WHERE supplier.name = source.name);


-- -----------------------------------------------------------------------------
-- 4. Estoque: grupos e itens (as três categorias vêm do seed da API)
-- -----------------------------------------------------------------------------
INSERT INTO stock_group (name, stock_category_id)
SELECT source.name, category.id
FROM (SELECT 'Cimento e argamassa' AS name, 'Consumable material' AS category
      UNION ALL SELECT 'Aço', 'Consumable material'
      UNION ALL SELECT 'Agregados', 'Consumable material'
      UNION ALL SELECT 'Alvenaria', 'Consumable material'
      UNION ALL SELECT 'Proteção individual', 'PPE'
      UNION ALL SELECT 'Equipamentos', 'Tool') AS source
JOIN stock_category AS category ON category.name = source.category
WHERE NOT EXISTS (SELECT 1 FROM stock_group
                  WHERE stock_group.name = source.name AND stock_group.stock_category_id = category.id);

DROP TEMPORARY TABLE IF EXISTS seed_stock_item;
CREATE TEMPORARY TABLE seed_stock_item (
    code VARCHAR(30), name VARCHAR(160), stock_group VARCHAR(120), unit VARCHAR(30),
    min_quantity DECIMAL(15,3), reference_price DECIMAL(15,2), ca VARCHAR(20), ca_expiry_days INT);
INSERT INTO seed_stock_item VALUES
    ('CIM-50',  'Cimento CP-II 50 kg',          'Cimento e argamassa', 'saco',  100,   38.00, NULL, NULL),
    ('ARG-20',  'Argamassa AC-II 20 kg',        'Cimento e argamassa', 'saco',   50,   22.00, NULL, NULL),
    ('ACO-10',  'Vergalhão CA-50 10 mm',        'Aço',                 'barra',  80,   49.00, NULL, NULL),
    ('ARE-M',   'Areia média',                  'Agregados',           'm³',     10,  130.00, NULL, NULL),
    ('BRI-1',   'Brita 1',                      'Agregados',           'm³',     10,  145.00, NULL, NULL),
    ('BLO-14',  'Bloco cerâmico 14x19x29',      'Alvenaria',           'un',   2000,    1.35, NULL, NULL),
    ('CAP-B',   'Capacete classe B',            'Proteção individual', 'un',     20,   42.00, '31469',  400),
    ('BOT-01',  'Botina de segurança',          'Proteção individual', 'par',    15,  119.00, '42049',  -10),
    ('LUV-01',  'Luva de vaqueta',              'Proteção individual', 'par',    40,   18.50, '35270',  200),
    ('OCU-01',  'Óculos de proteção incolor',   'Proteção individual', 'un',     30,   12.90, '19176',   20),
    ('BET-400', 'Betoneira 400 L',              'Equipamentos',        'un',      1, 4900.00, NULL, NULL),
    ('FUR-01',  'Furadeira de impacto',         'Equipamentos',        'un',      2,  690.00, NULL, NULL),
    ('SER-01',  'Serra circular',               'Equipamentos',        'un',      2,  820.00, NULL, NULL);

INSERT INTO stock_item (stock_group_id, code, name, unit, min_quantity, reference_price, ca, ca_expiry_date, active)
SELECT stock_group.id, source.code, source.name, source.unit, source.min_quantity, source.reference_price,
       source.ca, @today + INTERVAL source.ca_expiry_days DAY, 1
FROM seed_stock_item AS source
JOIN stock_group ON stock_group.name = source.stock_group
WHERE NOT EXISTS (SELECT 1 FROM stock_item WHERE stock_item.code = source.code OR stock_item.name = source.name);


-- -----------------------------------------------------------------------------
-- 5. Treinamentos (NR) e certificados: um vencido, dois vencendo, o resto em dia
-- -----------------------------------------------------------------------------
INSERT INTO training_type (code, name, validity_months, min_workload_hours, requires_in_person)
SELECT source.code, source.name, source.validity, source.workload, source.in_person
FROM (SELECT 'NR-35' AS code, 'Trabalho em altura' AS name, 24 AS validity, 8 AS workload, 1 AS in_person
      UNION ALL SELECT 'NR-18', 'Segurança na construção civil', 24, 4, 1
      UNION ALL SELECT 'NR-06', 'Equipamento de proteção individual', 12, 2, 0
      UNION ALL SELECT 'NR-10', 'Segurança em instalações elétricas', 24, 40, 1
      UNION ALL SELECT 'NR-12', 'Segurança em máquinas e equipamentos', 24, 8, 1) AS source
WHERE NOT EXISTS (SELECT 1 FROM training_type WHERE training_type.code = source.code);

DROP TEMPORARY TABLE IF EXISTS seed_training;
CREATE TEMPORARY TABLE seed_training (registration VARCHAR(20), training VARCHAR(20), issued_days_ago INT, expires_in_days INT);
INSERT INTO seed_training VALUES
    ('MAT-1001', 'NR-35', 740,  -10),   -- vencido
    ('MAT-1001', 'NR-18', 300,  430),
    ('MAT-1002', 'NR-35', 100,  630),
    ('MAT-1004', 'NR-35', 715,   15),   -- vencendo
    ('MAT-1004', 'NR-18', 200,  530),
    ('MAT-1005', 'NR-18', 690,   40),
    ('MAT-1007', 'NR-35', 400,  330),
    ('MAT-1009', 'NR-10', 710,   20),   -- vencendo
    ('MAT-1023', 'NR-10', 120,  610),
    ('MAT-1008', 'NR-12', 150,  580),
    ('MAT-1003', 'NR-06',  60,  305);

INSERT INTO employee_training (employee_id, training_type_id, issue_date, expiry_date, workload_hours, modality, instructor)
SELECT employee.id, training_type.id, @today - INTERVAL source.issued_days_ago DAY,
       @today + INTERVAL source.expires_in_days DAY, training_type.min_workload_hours, 'in_person', 'SESMT NoPrumo'
FROM seed_training AS source
JOIN seed_employee_id AS employee ON employee.registration = source.registration
JOIN training_type ON training_type.code = source.training
WHERE NOT EXISTS (SELECT 1 FROM employee_training
                  WHERE employee_training.employee_id = employee.id AND employee_training.training_type_id = training_type.id);


-- -----------------------------------------------------------------------------
-- 6. Equipes
-- -----------------------------------------------------------------------------
INSERT INTO team (name, department_id)
SELECT source.name, department.id
FROM (SELECT 'Equipe Estrutura' AS name UNION ALL SELECT 'Equipe Alvenaria' UNION ALL SELECT 'Equipe Elétrica') AS source
JOIN department ON department.name = 'Obras'
WHERE NOT EXISTS (SELECT 1 FROM team WHERE team.name = source.name AND team.deleted_at IS NULL);

DROP TEMPORARY TABLE IF EXISTS seed_member;
CREATE TEMPORARY TABLE seed_member (team VARCHAR(120), registration VARCHAR(20));
INSERT INTO seed_member VALUES
    ('Equipe Estrutura', 'MAT-1003'), ('Equipe Estrutura', 'MAT-1007'), ('Equipe Estrutura', 'MAT-1008'),
    ('Equipe Estrutura', 'MAT-1013'), ('Equipe Estrutura', 'MAT-1010'),
    ('Equipe Alvenaria', 'MAT-1004'), ('Equipe Alvenaria', 'MAT-1005'), ('Equipe Alvenaria', 'MAT-1024'),
    ('Equipe Alvenaria', 'MAT-1006'), ('Equipe Alvenaria', 'MAT-1011'), ('Equipe Alvenaria', 'MAT-1012'),
    ('Equipe Elétrica', 'MAT-1009'), ('Equipe Elétrica', 'MAT-1023');

INSERT INTO employee_team (employee_id, team_id, start_date)
SELECT employee.id, team.id, @today - INTERVAL 240 DAY
FROM seed_member AS source
JOIN seed_employee_id AS employee ON employee.registration = source.registration
JOIN team ON team.name = source.team AND team.deleted_at IS NULL
WHERE NOT EXISTS (SELECT 1 FROM employee_team
                  WHERE employee_team.employee_id = employee.id AND employee_team.team_id = team.id);


-- -----------------------------------------------------------------------------
-- 7. Obras e etapas (datas contadas a partir de hoje)
-- -----------------------------------------------------------------------------
DROP TEMPORARY TABLE IF EXISTS seed_project;
CREATE TEMPORARY TABLE seed_project (
    code VARCHAR(30), name VARCHAR(160), client VARCHAR(160), supervisor VARCHAR(20), status VARCHAR(20),
    contract DECIMAL(15,2), start_offset INT, forecast_offset INT, completion_offset INT,
    district VARCHAR(80), technical_manager VARCHAR(120), crea VARCHAR(30));
INSERT INTO seed_project VALUES
    ('OB-2025-07', 'Reforma E. M. Bela Vista',  'Prefeitura de Blumenau',      'MAT-1002', 'completed',    640000.00, -300,  -90,  -95, 'Velha',       'Patrícia Koch',  'SC-112233-4'),
    ('OB-2026-01', 'Residencial Aurora',        'Aurora Incorporadora Ltda',   'MAT-1001', 'in_progress', 2480000.00, -220,  140, NULL, 'Victor Konder','Patrícia Koch',  'SC-112233-4'),
    ('OB-2026-02', 'Galpão Logístico Itoupava', 'Itoupava Logística S.A.',     'MAT-1002', 'in_progress', 1365000.00, -150,   50, NULL, 'Itoupava Central', 'Fernando Lenzi', 'SC-445566-7'),
    ('OB-2026-03', 'Edifício Ponta Aguda',      'Ponta Aguda Empreendimentos', NULL,       'planning',    7920000.00,   80,  800, NULL, 'Ponta Aguda', 'Patrícia Koch',  'SC-112233-4');

INSERT INTO project (code, name, client_id, supervisor_id, status, contract_amount, start_date, forecast_date,
                     completion_date, district, city, state, technical_manager, crea_rt)
SELECT source.code, source.name, client.id, supervisor.id, source.status, source.contract,
       @today + INTERVAL source.start_offset DAY, @today + INTERVAL source.forecast_offset DAY,
       @today + INTERVAL source.completion_offset DAY, source.district, 'Blumenau', 'SC',
       source.technical_manager, source.crea
FROM seed_project AS source
LEFT JOIN client ON client.name = source.client
LEFT JOIN seed_employee_id AS supervisor ON supervisor.registration = source.supervisor
WHERE NOT EXISTS (SELECT 1 FROM project WHERE project.code = source.code AND project.deleted_at IS NULL);

DROP TEMPORARY TABLE IF EXISTS seed_stage;
CREATE TEMPORARY TABLE seed_stage (
    project VARCHAR(30), sort_order INT, name VARCHAR(120), team VARCHAR(120),
    planned_offset INT, percentage DECIMAL(5,2), status VARCHAR(20));
INSERT INTO seed_stage VALUES
    ('OB-2025-07', 1, 'Demolição e retirada',          'Equipe Alvenaria', -270, 100, 'completed'),
    ('OB-2025-07', 2, 'Cobertura',                     'Equipe Estrutura', -200, 100, 'completed'),
    ('OB-2025-07', 3, 'Instalações elétricas',         'Equipe Elétrica',  -150, 100, 'completed'),
    ('OB-2025-07', 4, 'Pintura e acabamento',          'Equipe Alvenaria', -100, 100, 'completed'),
    ('OB-2026-01', 1, 'Fundação',                      'Equipe Estrutura', -170, 100, 'completed'),
    ('OB-2026-01', 2, 'Estrutura',                     'Equipe Estrutura',  -30, 100, 'completed'),
    ('OB-2026-01', 3, 'Alvenaria',                     'Equipe Alvenaria',   25,  65, 'in_progress'),
    ('OB-2026-01', 4, 'Instalações',                   'Equipe Elétrica',    70,  30, 'in_progress'),
    ('OB-2026-01', 5, 'Revestimento',                  'Equipe Alvenaria',  105,   0, 'planned'),
    ('OB-2026-01', 6, 'Acabamento',                    'Equipe Alvenaria',  135,   0, 'planned'),
    ('OB-2026-02', 1, 'Terraplenagem',                 'Equipe Estrutura', -120, 100, 'completed'),
    ('OB-2026-02', 2, 'Fundação',                      'Equipe Estrutura',  -60,  90, 'in_progress'),
    ('OB-2026-02', 3, 'Estrutura metálica',            'Equipe Estrutura',  -12,  20, 'in_progress'),
    ('OB-2026-02', 4, 'Piso industrial',               'Equipe Alvenaria',   30,   0, 'planned'),
    ('OB-2026-02', 5, 'Instalações elétricas',         'Equipe Elétrica',    45,   0, 'planned'),
    ('OB-2026-03', 1, 'Projeto executivo',             NULL,                 60,   0, 'planned'),
    ('OB-2026-03', 2, 'Fundação',                      'Equipe Estrutura',  160,   0, 'planned');

INSERT INTO stage (project_id, sort_order, name, team_id, supervisor_id, planned_date, completion_date, percentage, status)
SELECT project.id, source.sort_order, source.name, team.id, project.supervisor_id,
       @today + INTERVAL source.planned_offset DAY,
       IF(source.status = 'completed', @today + INTERVAL (source.planned_offset - 3) DAY, NULL),
       source.percentage, source.status
FROM seed_stage AS source
JOIN project ON project.code = source.project AND project.deleted_at IS NULL
LEFT JOIN team ON team.name = source.team AND team.deleted_at IS NULL
WHERE NOT EXISTS (SELECT 1 FROM stage WHERE stage.project_id = project.id AND stage.name = source.name);

INSERT INTO team_project (team_id, project_id, start_date, end_date)
SELECT team.id, project.id, @today + INTERVAL source.start_offset DAY,
       IF(source.end_offset IS NULL, NULL, @today + INTERVAL source.end_offset DAY)
FROM (SELECT 'Equipe Estrutura' AS team, 'OB-2026-01' AS project, -200 AS start_offset, NULL AS end_offset
      UNION ALL SELECT 'Equipe Alvenaria', 'OB-2026-01', -60, NULL
      UNION ALL SELECT 'Equipe Elétrica',  'OB-2026-02', -40, NULL
      UNION ALL SELECT 'Equipe Alvenaria', 'OB-2025-07', -290, -95) AS source
JOIN team ON team.name = source.team AND team.deleted_at IS NULL
JOIN project ON project.code = source.project AND project.deleted_at IS NULL
WHERE NOT EXISTS (SELECT 1 FROM team_project
                  WHERE team_project.team_id = team.id AND team_project.project_id = project.id);


-- -----------------------------------------------------------------------------
-- 8. Movimento: apaga o 'demo' anterior e gera de novo a partir de hoje
-- -----------------------------------------------------------------------------
DELETE FROM time_entry WHERE source = 'demo';
DELETE FROM stock_movement WHERE source_type = 'demo';
DELETE FROM project_amendment WHERE number LIKE 'DEMO-%';

-- Quem trabalha em qual obra. O período é o da obra: do início até a conclusão ou ontem.
DROP TEMPORARY TABLE IF EXISTS seed_crew;
CREATE TEMPORARY TABLE seed_crew (registration VARCHAR(20), project VARCHAR(30));
INSERT INTO seed_crew VALUES
    ('MAT-1001', 'OB-2026-01'),
    ('MAT-1003', 'OB-2026-01'),
    ('MAT-1004', 'OB-2026-01'),
    ('MAT-1005', 'OB-2026-01'),
    ('MAT-1006', 'OB-2026-01'),
    ('MAT-1007', 'OB-2026-01'),
    ('MAT-1010', 'OB-2026-01'),
    ('MAT-1024', 'OB-2026-01'),
    ('MAT-1002', 'OB-2026-02'),
    ('MAT-1008', 'OB-2026-02'),
    ('MAT-1009', 'OB-2026-02'),
    ('MAT-1011', 'OB-2026-02'),
    ('MAT-1013', 'OB-2026-02'),
    ('MAT-1023', 'OB-2026-02'),
    ('MAT-1012', 'OB-2025-07'),
    ('MAT-1024', 'OB-2025-07'),
    ('MAT-1009', 'OB-2025-07');

-- Ponto: dias úteis, saída variando entre 16:30 e 18:00, uma falta de vez em
-- quando e sábado de meio período alternado na Aurora. O custo segue a regra do
-- EmploymentRegime.HourlyRate e do TimeEntry.CalculateCost.
INSERT INTO time_entry (employee_id, project_id, date, clock_in, break_start, break_end, clock_out,
                        hourly_rate_snapshot, additional_snapshot, charges_snapshot, cost, source)
WITH RECURSIVE calendar (day) AS (
    SELECT @first_day
    UNION ALL
    SELECT day + INTERVAL 1 DAY FROM calendar WHERE day < @yesterday
),
shift AS (
    SELECT employee.id AS employee_id,
           project.id AS project_id,
           calendar.day,
           DAYOFWEEK(calendar.day) = 7 AS saturday,
           CRC32(CONCAT(calendar.day, '-', employee.id)) AS noise,
           employee.pay_rate,
           employee.additional_percentage,
           regime.id AS regime_id,
           regime.unit,
           regime.monthly_hours
    FROM seed_crew AS crew
    JOIN seed_employee_id AS crew_member ON crew_member.registration = crew.registration
    JOIN employee ON employee.id = crew_member.id
    JOIN employment_regime AS regime ON regime.id = employee.employment_regime_id
    JOIN project ON project.code = crew.project AND project.deleted_at IS NULL
    JOIN calendar ON calendar.day BETWEEN project.start_date
                                      AND LEAST(COALESCE(project.completion_date, @yesterday), @yesterday)
    WHERE DAYOFWEEK(calendar.day) BETWEEN 2 AND 6
       OR (DAYOFWEEK(calendar.day) = 7 AND crew.project = 'OB-2026-01' AND WEEK(calendar.day) % 2 = 0)
),
worked AS (
    SELECT shift.*,
           TIME('07:00') AS clock_in,
           IF(saturday, NULL, TIME('12:00')) AS break_start,
           IF(saturday, NULL, TIME('13:00')) AS break_end,
           IF(saturday, TIME('11:00'), ADDTIME('16:30', SEC_TO_TIME((noise % 7) * 900))) AS clock_out
    FROM shift
    WHERE noise % 13 <> 0
),
priced AS (
    SELECT worked.*,
           CASE unit
               WHEN 'month' THEN pay_rate / COALESCE(monthly_hours, 220)
               WHEN 'day'   THEN pay_rate / 8
               ELSE pay_rate
           END AS hourly_rate,
           COALESCE((SELECT charge.percentage FROM payroll_charge_rate AS charge
                     WHERE charge.employment_regime_id = worked.regime_id
                       AND charge.effective_start <= worked.day
                       AND (charge.effective_end IS NULL OR charge.effective_end >= worked.day)
                     ORDER BY charge.effective_start DESC LIMIT 1),
                    -- O seed da API grava os encargos com início no dia em que subiu;
                    -- para o passado, vale a taxa mais antiga do regime.
                    (SELECT charge.percentage FROM payroll_charge_rate AS charge
                     WHERE charge.employment_regime_id = worked.regime_id
                     ORDER BY charge.effective_start LIMIT 1),
                    0) AS charges,
           ROUND((TIME_TO_SEC(TIMEDIFF(clock_out, clock_in))
                  - IFNULL(TIME_TO_SEC(TIMEDIFF(break_end, break_start)), 0)) / 3600, 2) AS hours
    FROM worked
)
SELECT employee_id, project_id, day, clock_in, break_start, break_end, clock_out,
       ROUND(hourly_rate, 4), additional_percentage, charges,
       ROUND(hours * hourly_rate * (1 + additional_percentage / 100) * (1 + charges / 100), 2),
       'demo'
FROM priced;

-- Material: entregas em dias úteis, com o preço da nota oscilando entre 95% e
-- 110% do preço de referência. scale = tamanho da obra.
DROP TEMPORARY TABLE IF EXISTS seed_delivery_plan;
CREATE TEMPORARY TABLE seed_delivery_plan (
    project VARCHAR(30), scale DECIMAL(4,2));
INSERT INTO seed_delivery_plan VALUES
    ('OB-2026-01', 1.00),
    ('OB-2026-02', 0.90),
    ('OB-2025-07', 0.65);

DROP TEMPORARY TABLE IF EXISTS seed_material;
CREATE TEMPORARY TABLE seed_material (
    code VARCHAR(30), supplier VARCHAR(160), base_quantity INT, spread INT, every_days INT);
INSERT INTO seed_material VALUES
    ('CIM-50', 'Cimentos Vale do Itajaí Ltda', 150, 220, 3),
    ('ARG-20', 'Cimentos Vale do Itajaí Ltda',  75, 125, 6),
    ('ACO-10', 'Aço Forte Distribuidora Ltda', 150, 350, 5),
    ('ARE-M',  'Areal Itajaí-Açu Ltda',         20,  30, 4),
    ('BRI-1',  'Areal Itajaí-Açu Ltda',         15,  25, 5),
    ('BLO-14', 'Cerâmica Rio do Sul',        7500, 10000, 7);

INSERT INTO stock_movement (stock_item_id, project_id, type, quantity, unit, unit_cost, supplier_id,
                            invoice_number, source_type, notes, date)
WITH RECURSIVE calendar (day) AS (
    SELECT @first_day
    UNION ALL
    SELECT day + INTERVAL 1 DAY FROM calendar WHERE day < @yesterday
),
delivery AS (
    SELECT calendar.day,
           project.id AS project_id,
           plan.scale,
           item.id AS stock_item_id,
           item.unit,
           item.reference_price,
           supplier.id AS supplier_id,
           material.base_quantity,
           material.spread,
           material.every_days,
           CRC32(CONCAT(calendar.day, '-', project.id, '-', item.id)) AS noise
    FROM seed_delivery_plan AS plan
    JOIN project ON project.code = plan.project AND project.deleted_at IS NULL
    JOIN calendar ON calendar.day BETWEEN project.start_date
                                      AND LEAST(COALESCE(project.completion_date, @yesterday), @yesterday)
    JOIN seed_material AS material
    JOIN stock_item AS item ON item.code = material.code
    LEFT JOIN supplier ON supplier.name = material.supplier
    WHERE DAYOFWEEK(calendar.day) BETWEEN 2 AND 6
)
SELECT stock_item_id, project_id, 'entry',
       ROUND((base_quantity + noise % spread) * scale),
       unit,
       ROUND(reference_price * (0.95 + (noise % 16) / 100), 2),
       supplier_id,
       CONCAT('NF-', 40000 + noise % 60000),
       'demo', 'demo',
       day + INTERVAL 9 HOUR
FROM delivery
WHERE noise % every_days = 0;

-- Consumo: dois dias depois de cada entrega sai 80% do que chegou, então
-- sobra saldo na obra para a aba Material mostrar.
INSERT INTO stock_movement (stock_item_id, project_id, type, quantity, unit, unit_cost, source_type, notes, date)
SELECT stock_item_id, project_id, 'exit', ROUND(quantity * 0.8), unit, 0, 'demo', 'demo', date + INTERVAL 2 DAY
FROM stock_movement
WHERE source_type = 'demo' AND type = 'entry' AND date + INTERVAL 2 DAY < @today;

-- Um aditivo de valor na Aurora.
INSERT INTO project_amendment (project_id, type, number, date, amount, extra_days, reason)
SELECT project.id, 'value', 'DEMO-001', @today - INTERVAL 30 DAY, 85000.00, 20,
       'Troca do revestimento da fachada por porcelanato, a pedido do cliente.'
FROM project WHERE project.code = 'OB-2026-01' AND project.deleted_at IS NULL;


-- -----------------------------------------------------------------------------
-- Conferência
-- -----------------------------------------------------------------------------
SELECT project.code,
       (SELECT COUNT(*) FROM time_entry WHERE time_entry.project_id = project.id AND source = 'demo') AS pontos,
       (SELECT ROUND(SUM(cost)) FROM time_entry WHERE time_entry.project_id = project.id) AS mao_de_obra,
       (SELECT ROUND(SUM(quantity * unit_cost)) FROM stock_movement
         WHERE stock_movement.project_id = project.id AND type = 'entry') AS material
FROM project
WHERE project.code IN ('OB-2025-07', 'OB-2026-01', 'OB-2026-02', 'OB-2026-03')
ORDER BY project.code;
