-- mysql -u root -p -e "source seed.sql"

-- caso não funcione, utilize o comando:

--  & "C:\Program Files\MySQL\MySQL Server 8.0\bin\mysql.exe" -u root -p -e "source seed.sql"

-- By: Adrian


USE noprumodb;

-- Desativar verificações de chave estrangeira temporariamente para permitir o TRUNCATE
SET FOREIGN_KEY_CHECKS = 0;

-- Limpar as tabelas para evitar duplicações se correr o script várias vezes
TRUNCATE TABLE employment_regime;
TRUNCATE TABLE job_role;
TRUNCATE TABLE department;

-- Reativar verificações
SET FOREIGN_KEY_CHECKS = 1;

-- 1. Inserir Regimes de Contratação
INSERT INTO employment_regime (label, unit, monthly_hours, description, active) VALUES
('CLT Mensalista', 'month', 220.00, 'Regime CLT padrão com remuneração mensal', 1),
('CLT Horista', 'hour', NULL, 'Regime CLT com remuneração baseada em horas trabalhadas', 1),
('PJ Prestador de Serviço', 'month', NULL, 'Contrato de prestação de serviços pessoa jurídica', 1),
('Estágio', 'month', 120.00, 'Contrato de estágio meio período (6h/dia)', 1),
('Jovem Aprendiz', 'day', 100.00, 'Programa de aprendizagem e capacitação profissional', 1),
('Temporário (Lei 6.019)', 'month', 220.00, 'Contrato de trabalho temporário por prazo determinado', 1),
('Sócio/Diretor (Pró-labore)', 'month', NULL, 'Remuneração de sócios e diretores', 1);

-- 2. Inserir Setores (Departments)
INSERT INTO department (name, active) VALUES
('Engenharia Civil', 1),
('Engenharia Elétrica', 1),
('Recursos Humanos', 1),
('Financeiro', 1),
('Operações e Obras', 1),
('Comercial', 1),
('Tecnologia da Informação', 1),
('Suprimentos e Compras', 1),
('Segurança do Trabalho (SESMT)', 1),
('Logística e Frota', 1),
('Manutenção Pesada', 1),
('Almoxarifado Central', 1);

-- 3. Inserir Funções (Job Roles)
-- Os department_id correspondem à ordem dos INSERTS acima (1 a 12)
INSERT INTO job_role (name, department_id, active) VALUES
-- Engenharia Civil (1)
('Engenheiro Civil Sênior', 1, 1),
('Engenheiro Civil Pleno', 1, 1),
('Arquiteto Sênior', 1, 1),
('Cadista / Projetista', 1, 1),

-- Engenharia Elétrica (2)
('Engenheiro Eletricista', 2, 1),
('Técnico Eletrotécnico', 2, 1),
('Eletricista de Alta Tensão', 2, 1),

-- Recursos Humanos (3)
('Gerente de RH', 3, 1),
('Analista de Recursos Humanos', 3, 1),
('Assistente de Departamento Pessoal', 3, 1),
('Psicólogo Organizacional', 3, 1),

-- Financeiro (4)
('Diretor Financeiro (CFO)', 4, 1),
('Controller', 4, 1),
('Analista Financeiro Pleno', 4, 1),
('Assistente de Faturamento', 4, 1),

-- Operações e Obras (5)
('Mestre de Obras', 5, 1),
('Encarregado de Obras', 5, 1),
('Pedreiro Oficial', 5, 1),
('Carpinteiro', 5, 1),
('Armador', 5, 1),
('Servente de Obras', 5, 1),

-- Comercial (6)
('Gerente Comercial', 6, 1),
('Orçamentista Sênior', 6, 1),
('Vendedor Técnico', 6, 1),

-- TI (7)
('Desenvolvedor Full Stack', 7, 1),
('Analista de Suporte de TI', 7, 1),

-- Suprimentos (8)
('Comprador Sênior', 8, 1),
('Assistente de Compras', 8, 1),

-- Segurança do Trabalho (9)
('Engenheiro de Segurança do Trabalho', 9, 1),
('Técnico de Segurança do Trabalho', 9, 1),
('Médico do Trabalho', 9, 1),

-- Logística (10)
('Coordenador de Logística', 10, 1),
('Motorista de Caminhão Betoneira', 10, 1),
('Operador de Munck', 10, 1),

-- Manutenção (11)
('Mecânico de Máquinas Pesadas', 11, 1),
('Auxiliar de Manutenção', 11, 1),

-- Almoxarifado (12)
('Almoxarife', 12, 1),
('Auxiliar de Almoxarifado', 12, 1);

-- 4. Inserir 15 Funcionários (Garante que passa da página 1, já que o limite é 10)
-- Os job_role_id e employment_regime_id batem com os que inserimos no seed anterior!
INSERT INTO employee 
(registration_number, name, job_role_id, employment_regime_id, pay_rate, additional_percentage, hire_date, phone, document_hash, document_masked, document_encrypted, active) 
VALUES
('MAT-1001', 'Carlos Drummond de Andrade', 1, 1, 12500.00, 0.00, '2023-01-10', '47999990001', SHA2('MAT-1001', 256), '***.***.111-11', 'dummy_enc_data', 1),
('MAT-1002', 'Maria Antonieta das Neves', 2, 1, 8500.00, 0.00, '2023-02-15', '47999990002', SHA2('MAT-1002', 256), '***.***.222-22', 'dummy_enc_data', 1),
('MAT-1003', 'Machado de Assis', 3, 3, 14000.00, 0.00, '2022-11-05', '47999990003', SHA2('MAT-1003', 256), '***.***.333-33', 'dummy_enc_data', 1),
('MAT-1004', 'Clarice Lispector', 7, 1, 4500.00, 30.00, '2024-01-20', '47999990004', SHA2('MAT-1004', 256), '***.***.444-44', 'dummy_enc_data', 1), -- Eletricista com 30%
('MAT-1005', 'Jorge Amado', 8, 1, 9500.00, 0.00, '2021-05-10', '47999990005', SHA2('MAT-1005', 256), '***.***.555-55', 'dummy_enc_data', 1),
('MAT-1006', 'Cecília Meireles', 10, 1, 3500.00, 0.00, '2023-08-01', '47999990006', SHA2('MAT-1006', 256), '***.***.666-66', 'dummy_enc_data', 1),
('MAT-1007', 'João Guimarães Rosa', 12, 7, 25000.00, 0.00, '2020-02-01', '47999990007', SHA2('MAT-1007', 256), '***.***.777-77', 'dummy_enc_data', 1), -- Sócio/Diretor
('MAT-1008', 'Érico Veríssimo', 16, 1, 6500.00, 0.00, '2022-07-22', '47999990008', SHA2('MAT-1008', 256), '***.***.888-88', 'dummy_enc_data', 1),
('MAT-1009', 'Lygia Fagundes Telles', 18, 1, 3200.00, 10.00, '2024-03-15', '47999990009', SHA2('MAT-1009', 256), '***.***.999-99', 'dummy_enc_data', 1), -- Pedreiro com 10%
('MAT-1010', 'Monteiro Lobato', 21, 6, 2100.00, 0.00, '2024-05-01', '47999990010', SHA2('MAT-1010', 256), '***.***.000-00', 'dummy_enc_data', 1), -- Temporário
('MAT-1011', 'Graciliano Ramos', 24, 1, 4800.00, 0.00, '2023-11-11', '47999990011', SHA2('MAT-1011', 256), '***.***.123-45', 'dummy_enc_data', 1),
('MAT-1012', 'Rubem Alves', 25, 3, 11500.00, 0.00, '2022-09-09', '47999990012', SHA2('MAT-1012', 256), '***.***.987-65', 'dummy_enc_data', 1), -- Dev Full Stack PJ
('MAT-1013', 'Cora Coralina', 29, 1, 8200.00, 0.00, '2021-12-01', '47999990013', SHA2('MAT-1013', 256), '***.***.555-12', 'dummy_enc_data', 1),
('MAT-1014', 'José de Alencar', 33, 1, 3800.00, 0.00, '2023-04-18', '47999990014', SHA2('MAT-1014', 256), '***.***.444-21', 'dummy_enc_data', 1),
('MAT-1015', 'Aluísio Azevedo', 37, 1, 2800.00, 0.00, '2024-02-10', '47999990015', SHA2('MAT-1015', 256), '***.***.333-89', 'dummy_enc_data', 1);

-- 5. Funcionários adicionais para testes de paginação, filtros e edição
-- IDs de funções conforme a ordem dos 38 cargos cadastrados no seed.
-- Regimes: 1=CLT Mensalista, 2=CLT Horista, 3=PJ,
--          4=Estágio, 5=Jovem Aprendiz, 6=Temporário,
--          7=Sócio/Diretor.

INSERT INTO employee
(registration_number, name, job_role_id, employment_regime_id,
 pay_rate, additional_percentage, hire_date, phone,
 document_hash, document_masked, document_encrypted, active)
VALUES

-- Engenharia Civil
('MAT-1016', 'Ana Beatriz Martins', 1, 1, 13200.00, 0.00, '2022-06-13', '47999990016', SHA2('MAT-1016', 256), '***.***.101-16', 'dummy_enc_data', 1),
('MAT-1017', 'Pedro Henrique Costa', 2, 1, 9200.00, 0.00, '2023-09-04', '47999990017', SHA2('MAT-1017', 256), '***.***.102-17', 'dummy_enc_data', 1),
('MAT-1018', 'Fernanda Oliveira Lima', 4, 1, 4600.00, 0.00, '2024-02-19', '47999990018', SHA2('MAT-1018', 256), '***.***.103-18', 'dummy_enc_data', 1),

-- Engenharia Elétrica
('MAT-1019', 'Rafael Mendes Rocha', 5, 1, 10800.00, 0.00, '2021-07-12', '47999990019', SHA2('MAT-1019', 256), '***.***.104-19', 'dummy_enc_data', 1),
('MAT-1020', 'Lucas Ferreira Alves', 6, 1, 5200.00, 0.00, '2023-03-06', '47999990020', SHA2('MAT-1020', 256), '***.***.105-20', 'dummy_enc_data', 1),
('MAT-1021', 'Bruno Carvalho Dias', 7, 1, 4900.00, 30.00, '2022-10-17', '47999990021', SHA2('MAT-1021', 256), '***.***.106-21', 'dummy_enc_data', 1),

-- Recursos Humanos
('MAT-1022', 'Juliana Ribeiro Souza', 9, 1, 5800.00, 0.00, '2023-01-16', '47999990022', SHA2('MAT-1022', 256), '***.***.107-22', 'dummy_enc_data', 1),
('MAT-1023', 'Camila Fernandes Moraes', 10, 1, 3300.00, 0.00, '2024-04-08', '47999990023', SHA2('MAT-1023', 256), '***.***.108-23', 'dummy_enc_data', 1),
('MAT-1024', 'Mariana Lopes Teixeira', 11, 1, 6100.00, 0.00, '2022-08-22', '47999990024', SHA2('MAT-1024', 256), '***.***.109-24', 'dummy_enc_data', 1),

-- Financeiro
('MAT-1025', 'Roberto Almeida Castro', 13, 1, 14500.00, 0.00, '2020-05-11', '47999990025', SHA2('MAT-1025', 256), '***.***.110-25', 'dummy_enc_data', 1),
('MAT-1026', 'Patrícia Gomes Freitas', 14, 1, 7200.00, 0.00, '2023-06-05', '47999990026', SHA2('MAT-1026', 256), '***.***.111-26', 'dummy_enc_data', 1),
('MAT-1027', 'André Luiz Barbosa', 15, 1, 3650.00, 0.00, '2024-01-08', '47999990027', SHA2('MAT-1027', 256), '***.***.112-27', 'dummy_enc_data', 1),

-- Operações e Obras
('MAT-1028', 'Marcos Vinícius Pereira', 17, 1, 5600.00, 0.00, '2021-02-15', '47999990028', SHA2('MAT-1028', 256), '***.***.113-28', 'dummy_enc_data', 1),
('MAT-1029', 'Eduardo Santos Nunes', 19, 1, 3900.00, 10.00, '2022-12-12', '47999990029', SHA2('MAT-1029', 256), '***.***.114-29', 'dummy_enc_data', 1),
('MAT-1030', 'Felipe Augusto Ribeiro', 20, 1, 3700.00, 0.00, '2023-05-22', '47999990030', SHA2('MAT-1030', 256), 'dummy_enc_data', 1),
('MAT-1031', 'Diego Martins Cardoso', 21, 6, 2300.00, 0.00, '2025-01-06', '47999990031', SHA2('MAT-1031', 256), '***.***.115-31', 'dummy_enc_data', 1),

-- Comercial
('MAT-1032', 'Ricardo Tavares Melo', 22, 1, 11800.00, 0.00, '2020-09-14', '47999990032', SHA2('MAT-1032', 256), '***.***.116-32', 'dummy_enc_data', 1),
('MAT-1033', 'Beatriz Carvalho Pinto', 23, 1, 6500.00, 0.00, '2022-04-04', '47999990033', SHA2('MAT-1033', 256), '***.***.117-33', 'dummy_enc_data', 1),

-- Tecnologia da Informação
('MAT-1034', 'Gabriel Moreira Silva', 25, 3, 12500.00, 0.00, '2023-07-10', '47999990034', SHA2('MAT-1034', 256), '***.***.118-34', 'dummy_enc_data', 1),
('MAT-1035', 'Thiago Correia Batista', 26, 1, 4200.00, 0.00, '2024-03-11', '47999990035', SHA2('MAT-1035', 256), '***.***.119-35', 'dummy_enc_data', 1),

-- Suprimentos e Compras
('MAT-1036', 'Débora Martins Araújo', 27, 1, 7800.00, 0.00, '2021-11-08', '47999990036', SHA2('MAT-1036', 256), '***.***.120-36', 'dummy_enc_data', 1),
('MAT-1037', 'Vinícius Lopes Mendes', 28, 1, 3400.00, 0.00, '2024-06-17', '47999990037', SHA2('MAT-1037', 256), '***.***.121-37', 'dummy_enc_data', 1),

-- Segurança do Trabalho
('MAT-1038', 'Sérgio Augusto Ramos', 30, 1, 4800.00, 0.00, '2022-01-10', '47999990038', SHA2('MAT-1038', 256), '***.***.122-38', 'dummy_enc_data', 1),
('MAT-1039', 'Aline Cristina Duarte', 31, 1, 14500.00, 0.00, '2020-08-03', '47999990039', SHA2('MAT-1039', 256), '***.***.123-39', 'dummy_enc_data', 1),

-- Logística e Frota
('MAT-1040', 'Alexandre Ribeiro Campos', 32, 1, 6200.00, 0.00, '2021-03-15', '47999990040', SHA2('MAT-1040', 256), '***.***.124-40', 'dummy_enc_data', 1),
('MAT-1041', 'Paulo Roberto Vieira', 34, 1, 4400.00, 0.00, '2023-10-02', '47999990041', SHA2('MAT-1041', 256), '***.***.125-41', 'dummy_enc_data', 1),

-- Manutenção Pesada
('MAT-1042', 'Wellington Souza Braga', 35, 1, 5900.00, 0.00, '2022-02-21', '47999990042', SHA2('MAT-1042', 256), '***.***.126-42', 'dummy_enc_data', 1),
('MAT-1043', 'Renan Oliveira Farias', 36, 1, 2900.00, 0.00, '2024-05-13', '47999990043', SHA2('MAT-1043', 256), '***.***.127-43', 'dummy_enc_data', 1),

-- Almoxarifado
('MAT-1044', 'Cristiano Mendes Lopes', 38, 1, 2600.00, 0.00, '2023-12-04', '47999990044', SHA2('MAT-1044', 256), '***.***.128-44', 'dummy_enc_data', 1),

-- Estágio e aprendizagem
('MAT-1045', 'Isabela Martins Cunha', 25, 4, 1500.00, 0.00, '2025-02-03', '47999990045', SHA2('MAT-1045', 256), '***.***.129-45', 'dummy_enc_data', 1),
('MAT-1046', 'Gustavo Henrique Leal', 26, 5, 1050.00, 0.00, '2025-03-10', '47999990046', SHA2('MAT-1046', 256), '***.***.130-46', 'dummy_enc_data', 1),

-- Contratos por hora e PJ
('MAT-1047', 'Daniela Costa Pires', 4, 2, 38.00, 0.00, '2024-08-12', '47999990047', SHA2('MAT-1047', 256), '***.***.131-47', 'dummy_enc_data', 1),
('MAT-1048', 'Leonardo Freitas Machado', 25, 3, 13800.00, 0.00, '2024-07-01', '47999990048', SHA2('MAT-1048', 256), '***.***.132-48', 'dummy_enc_data', 1),

-- Sócio/Diretor
('MAT-1049', 'Alexandre Monteiro Duarte', 12, 7, 28000.00, 0.00, '2019-01-07', '47999990049', SHA2('MAT-1049', 256), '***.***.133-49', 'dummy_enc_data', 1),

-- Funcionário inativo para testar filtros
('MAT-1050', 'Renata Alves Figueiredo', 14, 1, 6900.00, 0.00, '2020-06-01', '47999990050', SHA2('MAT-1050', 256), '***.***.134-50', 'dummy_enc_data', 0);
