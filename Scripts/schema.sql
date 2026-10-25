-- ============================================================
-- StudyToTech — Schema MySQL (PIM IV)
-- Alinhado ao EER Diagram e ao contrato OpenAPI
-- ============================================================

CREATE DATABASE IF NOT EXISTS studytotech
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;

USE studytotech;

-- ---------- perfil ----------
CREATE TABLE IF NOT EXISTS perfil (
  idperfil INT AUTO_INCREMENT PRIMARY KEY,
  nome     VARCHAR(45) NOT NULL
) ENGINE=InnoDB;

INSERT IGNORE INTO perfil (idperfil, nome) VALUES
  (1, 'Cliente'),
  (2, 'Funcionário'),
  (3, 'Administrador');

-- ---------- usuario ----------
CREATE TABLE IF NOT EXISTS usuario (
  idusuario       INT AUTO_INCREMENT PRIMARY KEY,
  nome            VARCHAR(100) NOT NULL,
  email           VARCHAR(100) NOT NULL,
  senha_hash      VARCHAR(255) NOT NULL,
  status          TINYINT(1)   NOT NULL DEFAULT 1,
  data_cadastro   DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
  perfil_idperfil INT          NOT NULL,
  UNIQUE KEY uk_usuario_email (email),
  CONSTRAINT fk_usuario_perfil
    FOREIGN KEY (perfil_idperfil) REFERENCES perfil (idperfil)
    ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB;

-- ---------- categoria ----------
CREATE TABLE IF NOT EXISTS categoria (
  idcategoria INT AUTO_INCREMENT PRIMARY KEY,
  nome        VARCHAR(45) NOT NULL
) ENGINE=InnoDB;

INSERT IGNORE INTO categoria (idcategoria, nome) VALUES
  (1, 'Computadores'),
  (2, 'Notebooks'),
  (3, 'Tablets'),
  (4, 'Acessórios');

-- ---------- produto ----------
CREATE TABLE IF NOT EXISTS produto (
  idproduto              INT AUTO_INCREMENT PRIMARY KEY,
  nome                   VARCHAR(100)  NOT NULL,
  descricao              VARCHAR(250)  NULL,
  categoria_idcategoria  INT           NOT NULL,
  preco                  DECIMAL(10,2) NOT NULL,
  estoque                INT           NOT NULL DEFAULT 0,
  estoque_minimo         INT           NOT NULL DEFAULT 0,
  status                 TINYINT(1)    NOT NULL DEFAULT 1,
  data_cadastro          DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_produto_categoria
    FOREIGN KEY (categoria_idcategoria) REFERENCES categoria (idcategoria)
    ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT chk_produto_estoque_nao_negativo CHECK (estoque >= 0)
) ENGINE=InnoDB;

-- ---------- status_pedido ----------
CREATE TABLE IF NOT EXISTS status_pedido (
  idstatus_pedido INT AUTO_INCREMENT PRIMARY KEY,
  nome            VARCHAR(45) NOT NULL
) ENGINE=InnoDB;

INSERT IGNORE INTO status_pedido (idstatus_pedido, nome) VALUES
  (1, 'Em elaboração'),
  (2, 'Finalizado'),
  (3, 'Em separação'),
  (4, 'Concluído'),
  (5, 'Cancelado');

-- ---------- pedido ----------
CREATE TABLE IF NOT EXISTS pedido (
  idpedido                INT AUTO_INCREMENT PRIMARY KEY,
  usuario_idusuario       INT      NOT NULL,
  status_idstatus_pedido  INT      NOT NULL,
  data_criacao            DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  data_finalizacao        DATETIME NULL,
  -- Coluna auxiliar para unicidade de "em elaboração" por cliente
  cliente_em_elaboracao   INT      NULL,
  UNIQUE KEY uk_pedido_cliente_em_elaboracao (cliente_em_elaboracao),
  CONSTRAINT fk_pedido_usuario
    FOREIGN KEY (usuario_idusuario) REFERENCES usuario (idusuario)
    ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT fk_pedido_status
    FOREIGN KEY (status_idstatus_pedido) REFERENCES status_pedido (idstatus_pedido)
    ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB;

-- ---------- item_pedido ----------
CREATE TABLE IF NOT EXISTS item_pedido (
  iditem_pedido   INT AUTO_INCREMENT PRIMARY KEY,
  pedido_idpedido INT           NOT NULL,
  produto_idproduto INT         NOT NULL,
  preco_unitario  DECIMAL(10,2) NOT NULL,
  quantidade      INT           NOT NULL,
  UNIQUE KEY uk_item_pedido_pedido_produto (pedido_idpedido, produto_idproduto),
  CONSTRAINT fk_item_pedido
    FOREIGN KEY (pedido_idpedido) REFERENCES pedido (idpedido)
    ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT fk_item_produto
    FOREIGN KEY (produto_idproduto) REFERENCES produto (idproduto)
    ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB;

-- ---------- movimentacao_estoque ----------
CREATE TABLE IF NOT EXISTS movimentacao_estoque (
  idmovimentacao_estoque INT AUTO_INCREMENT PRIMARY KEY,
  produto_idproduto      INT         NOT NULL,
  usuario_idusuario      INT         NOT NULL,
  tipo                   VARCHAR(10) NOT NULL,  -- ENTRADA | SAIDA
  quantidade             INT         NOT NULL,
  data_hora              DATETIME    NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_mov_produto
    FOREIGN KEY (produto_idproduto) REFERENCES produto (idproduto)
    ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT fk_mov_usuario
    FOREIGN KEY (usuario_idusuario) REFERENCES usuario (idusuario)
    ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB;

-- ---------- Views auxiliares ----------
CREATE OR REPLACE VIEW vw_produtos_estoque_baixo AS
SELECT
  p.idproduto   AS produto_id,
  p.nome,
  c.nome        AS categoria,
  p.estoque,
  p.estoque_minimo,
  (p.estoque < p.estoque_minimo) AS abaixo_do_minimo,
  p.status      AS ativo
FROM produto p
JOIN categoria c ON c.idcategoria = p.categoria_idcategoria
WHERE p.status = 1 AND p.estoque < p.estoque_minimo;

CREATE OR REPLACE VIEW vw_indicadores_admin AS
SELECT
  (SELECT COUNT(*) FROM produto) AS total_produtos,
  (SELECT COUNT(*) FROM produto WHERE status = 1 AND estoque < estoque_minimo) AS produtos_estoque_baixo,
  (SELECT COUNT(*) FROM pedido) AS total_pedidos,
  (SELECT COUNT(*) FROM pedido WHERE status_idstatus_pedido NOT IN (4, 5)) AS pedidos_pendentes;

-- ============================================================
-- Usuário admin padrão (senha: Admin@123) — BCrypt gerado em runtime
-- Preferencialmente criado pela própria API no startup.
-- ============================================================
