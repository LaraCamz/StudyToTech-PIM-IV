-- =====================================================
-- StudyToTech - PIM IV
-- =====================================================
-- Requer MySQL 8.0.16+.
-- =====================================================

SET FOREIGN_KEY_CHECKS = 0;

USE `mydb`;

-- =====================================================
-- TABELA: perfil
-- =====================================================

CREATE TABLE `perfil` (
    `idperfil` INT NOT NULL AUTO_INCREMENT,
    `nome` VARCHAR(45) NOT NULL,

    PRIMARY KEY (`idperfil`),
    UNIQUE KEY `uk_perfil_nome` (`nome`)
) ENGINE = InnoDB;


-- =====================================================
-- TABELA: usuario
-- =====================================================

CREATE TABLE `usuario` (
    `idusuario` INT NOT NULL AUTO_INCREMENT,
    `nome` VARCHAR(100) NOT NULL,
    `email` VARCHAR(100) NOT NULL,
    `senha_hash` VARCHAR(255) NOT NULL,
    `status` TINYINT(1) NOT NULL DEFAULT 1,
    `data_cadastro` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `perfil_idperfil` INT NOT NULL,

    PRIMARY KEY (`idusuario`),
    UNIQUE KEY `uk_usuario_email` (`email`),
    INDEX `idx_usuario_perfil` (`perfil_idperfil`),

    CONSTRAINT `fk_usuario_perfil`
        FOREIGN KEY (`perfil_idperfil`)
        REFERENCES `perfil` (`idperfil`)
        ON DELETE RESTRICT
        ON UPDATE CASCADE,

    CONSTRAINT `chk_usuario_status`
        CHECK (`status` IN (0, 1))
) ENGINE = InnoDB;


-- =====================================================
-- TABELA: categoria
-- =====================================================

CREATE TABLE `categoria` (
    `idcategoria` INT NOT NULL AUTO_INCREMENT,
    `nome` VARCHAR(45) NOT NULL,

    PRIMARY KEY (`idcategoria`),
    UNIQUE KEY `uk_categoria_nome` (`nome`)
) ENGINE = InnoDB;


-- =====================================================
-- TABELA: produto
-- =====================================================

CREATE TABLE `produto` (
    `idproduto` INT NOT NULL AUTO_INCREMENT,
    `nome` VARCHAR(100) NOT NULL,
    `descricao` VARCHAR(250) NULL,
    `categoria_idcategoria` INT NOT NULL,
    `preco` DECIMAL(10,2) NOT NULL,
    `estoque` INT NOT NULL DEFAULT 0,
    `estoque_minimo` INT NOT NULL DEFAULT 0,
    `status` TINYINT(1) NOT NULL DEFAULT 1,
    `data_cadastro` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    PRIMARY KEY (`idproduto`),
    INDEX `idx_produto_categoria` (`categoria_idcategoria`),
    INDEX `idx_produto_nome` (`nome`),

    CONSTRAINT `fk_produto_categoria`
        FOREIGN KEY (`categoria_idcategoria`)
        REFERENCES `categoria` (`idcategoria`)
        ON DELETE RESTRICT
        ON UPDATE CASCADE,

    CONSTRAINT `chk_produto_preco`
        CHECK (`preco` >= 0),

    CONSTRAINT `chk_produto_estoque`
        CHECK (`estoque` >= 0),

    CONSTRAINT `chk_produto_estoque_minimo`
        CHECK (`estoque_minimo` >= 0),

    CONSTRAINT `chk_produto_status`
        CHECK (`status` IN (0, 1))
) ENGINE = InnoDB;


-- =====================================================
-- TABELA: status_pedido
-- =====================================================

CREATE TABLE `status_pedido` (
    `idstatus_pedido` INT NOT NULL AUTO_INCREMENT,
    `nome` VARCHAR(45) NOT NULL,

    PRIMARY KEY (`idstatus_pedido`),
    UNIQUE KEY `uk_status_pedido_nome` (`nome`)
) ENGINE = InnoDB;


-- =====================================================
-- TABELA: pedido
-- =====================================================

CREATE TABLE `pedido` (
    `idpedido` INT NOT NULL AUTO_INCREMENT,
    `usuario_idusuario` INT NOT NULL,
    `status_idstatus_pedido` INT NOT NULL DEFAULT 1,
    `data_criacao` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `data_finalizacao` DATETIME NULL,

    PRIMARY KEY (`idpedido`),
    INDEX `idx_pedido_usuario` (`usuario_idusuario`),
    INDEX `idx_pedido_status` (`status_idstatus_pedido`),

    CONSTRAINT `fk_pedido_usuario`
        FOREIGN KEY (`usuario_idusuario`)
        REFERENCES `usuario` (`idusuario`)
        ON DELETE RESTRICT
        ON UPDATE CASCADE,

    CONSTRAINT `fk_pedido_status`
        FOREIGN KEY (`status_idstatus_pedido`)
        REFERENCES `status_pedido` (`idstatus_pedido`)
        ON DELETE RESTRICT
        ON UPDATE CASCADE
) ENGINE = InnoDB;


-- =====================================================
-- TABELA: item_pedido
-- =====================================================

CREATE TABLE `item_pedido` (
    `iditem_pedido` INT NOT NULL AUTO_INCREMENT,
    `pedido_idpedido` INT NOT NULL,
    `produto_idproduto` INT NOT NULL,
    `preco_unitario` DECIMAL(10,2) NOT NULL,
    `quantidade` INT NOT NULL,

    PRIMARY KEY (`iditem_pedido`),
    INDEX `idx_item_pedido_pedido` (`pedido_idpedido`),
    INDEX `idx_item_pedido_produto` (`produto_idproduto`),
    UNIQUE KEY `uk_item_pedido_pedido_produto` (`pedido_idpedido`, `produto_idproduto`),

    CONSTRAINT `fk_item_pedido_pedido`
        FOREIGN KEY (`pedido_idpedido`)
        REFERENCES `pedido` (`idpedido`)
        ON DELETE RESTRICT
        ON UPDATE CASCADE,

    CONSTRAINT `fk_item_pedido_produto`
        FOREIGN KEY (`produto_idproduto`)
        REFERENCES `produto` (`idproduto`)
        ON DELETE RESTRICT
        ON UPDATE CASCADE,

    CONSTRAINT `chk_item_pedido_quantidade`
        CHECK (`quantidade` > 0),

    CONSTRAINT `chk_item_pedido_preco`
        CHECK (`preco_unitario` >= 0)
) ENGINE = InnoDB;


-- =====================================================
-- TABELA: movimentacao_estoque
-- =====================================================

CREATE TABLE `movimentacao_estoque` (
    `idmovimentacao_estoque` INT NOT NULL AUTO_INCREMENT,
    `produto_idproduto` INT NOT NULL,
    `usuario_idusuario` INT NOT NULL,
    `tipo` VARCHAR(10) NOT NULL,
    `quantidade` INT NOT NULL,
    `data_hora` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    PRIMARY KEY (`idmovimentacao_estoque`),
    INDEX `idx_movimentacao_produto` (`produto_idproduto`),
    INDEX `idx_movimentacao_usuario` (`usuario_idusuario`),

    CONSTRAINT `fk_movimentacao_produto`
        FOREIGN KEY (`produto_idproduto`)
        REFERENCES `produto` (`idproduto`)
        ON DELETE RESTRICT
        ON UPDATE CASCADE,

    CONSTRAINT `fk_movimentacao_usuario`
        FOREIGN KEY (`usuario_idusuario`)
        REFERENCES `usuario` (`idusuario`)
        ON DELETE RESTRICT
        ON UPDATE CASCADE,

    CONSTRAINT `chk_movimentacao_tipo`
        CHECK (`tipo` IN ('ENTRADA', 'SAIDA')),

    CONSTRAINT `chk_movimentacao_quantidade`
        CHECK (`quantidade` > 0)
) ENGINE = InnoDB;


-- =====================================================
-- DADOS INICIAIS
-- =====================================================

INSERT INTO `perfil` (`nome`) VALUES
('Cliente'),
('Funcionário'),
('Administrador');

INSERT INTO `categoria` (`nome`) VALUES
('Computadores'),
('Notebooks'),
('Tablets'),
('Acessórios');

-- A ordem abaixo define os IDs usados nas triggers e na
-- procedure de finalização (1 = Em elaboração, 2 = Finalizado).
INSERT INTO `status_pedido` (`nome`) VALUES
('Em elaboração'),
('Finalizado'),
('Em separação'),
('Concluído'),
('Cancelado');


-- =====================================================
-- TRIGGERS
-- =====================================================

DELIMITER $$

-- RN03: impede adicionar item de produto inativo ou item
-- em um pedido que não esteja mais "Em elaboração".
CREATE TRIGGER `trg_item_pedido_before_insert`
BEFORE INSERT ON `item_pedido`
FOR EACH ROW
BEGIN
    DECLARE v_status_pedido INT;
    DECLARE v_produto_status TINYINT;

    SELECT `status_idstatus_pedido` INTO v_status_pedido
    FROM `pedido` WHERE `idpedido` = NEW.`pedido_idpedido`;

    SELECT `status` INTO v_produto_status
    FROM `produto` WHERE `idproduto` = NEW.`produto_idproduto`;

    IF v_status_pedido <> 1 THEN
        SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'Só é possível adicionar itens a pedidos com status "Em elaboração".';
    END IF;

    IF v_produto_status = 0 THEN
        SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'Não é possível adicionar um produto inativo ao pedido.';
    END IF;
END$$

-- RF10, RF13, RN01: impede registrar saída maior que o
-- estoque disponível (checagem antecipada, além da CHECK
-- em produto.estoque).
CREATE TRIGGER `trg_movimentacao_estoque_before_insert`
BEFORE INSERT ON `movimentacao_estoque`
FOR EACH ROW
BEGIN
    DECLARE v_estoque_atual INT;

    IF NEW.`tipo` = 'SAIDA' THEN
        SELECT `estoque` INTO v_estoque_atual
        FROM `produto` WHERE `idproduto` = NEW.`produto_idproduto`
        FOR UPDATE;

        IF v_estoque_atual < NEW.`quantidade` THEN
            SIGNAL SQLSTATE '45000'
                SET MESSAGE_TEXT = 'Estoque insuficiente para registrar esta saída.';
        END IF;
    END IF;
END$$

CREATE TRIGGER `trg_movimentacao_estoque_after_insert`
AFTER INSERT ON `movimentacao_estoque`
FOR EACH ROW
BEGIN
    IF NEW.`tipo` = 'ENTRADA' THEN
        UPDATE `produto` SET `estoque` = `estoque` + NEW.`quantidade`
        WHERE `idproduto` = NEW.`produto_idproduto`;
    ELSE
        UPDATE `produto` SET `estoque` = `estoque` - NEW.`quantidade`
        WHERE `idproduto` = NEW.`produto_idproduto`;
    END IF;
END$$

DELIMITER ;


-- =====================================================
-- PROCEDURE: sp_finalizar_pedido
-- =====================================================

DELIMITER $$

CREATE PROCEDURE `sp_finalizar_pedido`(IN p_idpedido INT)
BEGIN
    DECLARE v_insuficiente INT DEFAULT 0;

    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    START TRANSACTION;

    SELECT COUNT(*) INTO v_insuficiente
    FROM `item_pedido` ip
    JOIN `produto` p ON p.`idproduto` = ip.`produto_idproduto`
    WHERE ip.`pedido_idpedido` = p_idpedido
      AND p.`estoque` < ip.`quantidade`
    FOR UPDATE;

    IF v_insuficiente > 0 THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'Não é possível finalizar: um ou mais itens não têm estoque suficiente.';
    ELSE
        UPDATE `produto` p
        JOIN `item_pedido` ip ON ip.`produto_idproduto` = p.`idproduto`
        SET p.`estoque` = p.`estoque` - ip.`quantidade`
        WHERE ip.`pedido_idpedido` = p_idpedido;

        UPDATE `pedido`
        SET `status_idstatus_pedido` = 2, -- Finalizado
            `data_finalizacao` = NOW()
        WHERE `idpedido` = p_idpedido
          AND `status_idstatus_pedido` = 1; -- Em elaboração

        COMMIT;
    END IF;
END$$

DELIMITER ;


-- =====================================================
-- VIEWS DE APOIO
-- =====================================================

-- RF13: produtos com estoque abaixo do mínimo definido.
CREATE VIEW `vw_produtos_estoque_baixo` AS
SELECT
    p.`idproduto`,
    p.`nome`,
    p.`estoque`,
    p.`estoque_minimo`,
    c.`nome` AS `categoria`
FROM `produto` p
JOIN `categoria` c ON c.`idcategoria` = p.`categoria_idcategoria`
WHERE p.`estoque` < p.`estoque_minimo`
  AND p.`status` = 1;

-- RF25: indicadores básicos para o Administrador.
CREATE VIEW `vw_indicadores_admin` AS
SELECT
    (SELECT COUNT(*) FROM `produto`) AS `total_produtos`,
    (SELECT COUNT(*) FROM `produto` WHERE `estoque` < `estoque_minimo`) AS `produtos_estoque_baixo`,
    (SELECT COUNT(*) FROM `pedido`) AS `total_pedidos`,
    (
        SELECT COUNT(*)
        FROM `pedido` pd
        JOIN `status_pedido` sp ON sp.`idstatus_pedido` = pd.`status_idstatus_pedido`
        WHERE sp.`nome` NOT IN ('Concluído', 'Cancelado')
    ) AS `pedidos_pendentes`;


-- =====================================================
-- FINALIZAÇÃO
-- =====================================================

SET FOREIGN_KEY_CHECKS = 1;