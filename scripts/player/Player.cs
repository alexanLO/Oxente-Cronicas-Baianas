using Godot;
using System;

public partial class Player : CharacterBody2D
{

    private Vector2 velocity;
    private int jump_count; //* quantidade de pulos;

    [Export] private int Speed { get; set; } = 300; //* velocidade do andar do personagem
    [Export] private int PlayerGravity { get; set; } = 100; //* gravidade do personagem
    [Export] private int WallGravity { get; set; } = 100; //* gravidade do personagem
    [Export] private int JumpSpeed { get; set; } = -400; //* altura do pulo

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta); //* Chama o método _PhysicsProcess da classe base (CharacterBody2D)
        HorizontalMovimentEnv(); //* Gerencia o movimento horizontal do player
        VerticalMovimentEnv(); //* Gerencia o movimento vertical do player (pulo)
        Gravity(delta);
        MoveAndSlide(); //* Aplica o movimento e lida com as colisões
        Velocity = velocity; //* Atualiza a velocidade do corpo do player

    }

    private void Gravity(double delta)
    {
        velocity.Y += (float)delta * PlayerGravity;

        //* Limitador de velocidade de queda na vertical
        if (velocity.Y >= PlayerGravity)
        {
            velocity.Y = PlayerGravity;

        }
    }

    private void HorizontalMovimentEnv()
    {
        //* armazena a direção -1 para esquerda, 1 para direita e 0 para esteja parado ou aperte as duas teclas ao mesmo tempo.
        float input_direction = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");

        velocity.X = input_direction * Speed; //* Define a velocidade horizontal com base na entrada do jogador
    }

    private void VerticalMovimentEnv()
    {
        //* Condição para resetar o double jump
        if (IsOnFloor() || IsOnWall())
        {
            jump_count = 0; //* Reseta a contagem de pulos se o player está no chão ou na parede
        }

        //* Verificação se jogador deu 2 pulos ou se esta pressionando o pulo
        if (Input.IsActionJustPressed("jump") && jump_count < 2)
        {
            jump_count += 1; //* Incrementa a contagem de pulos
            velocity.Y = JumpSpeed; //* Aplica a velocidade do pulo normal
        }
    }

}
