-- =====================================================
-- StudyToTech - PIM IV
-- Banco de Dados
-- =====================================================

SET FOREIGN_KEY_CHECKS = 0;

DROP DATABASE IF EXISTS `mydb`;

CREATE DATABASE `mydb`
    DEFAULT CHARACTER SET utf8mb4
    DEFAULT COLLATE utf8mb4_unicode_ci;

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
        ON UPDATE CASCADE
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
        CHECK (`estoque_minimo` >= 0)
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
    `status_idstatus_pedido` INT NOT NULL,
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


INSERT INTO `status_pedido` (`nome`) VALUES
('Em elaboração'),
('Finalizado'),
('Em separação'),
('Concluído'),
('Cancelado');


-- =====================================================
-- FINALIZAÇÃO
-- =====================================================

SET FOREIGN_KEY_CHECKS = 1;