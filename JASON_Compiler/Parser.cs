using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JASON_Compiler
{
    public class Node
    {
        public List<Node> Children = new List<Node>();

        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }
    public class Parser
    {
        int InputPointer = 0;
        List<Token> TokenStream;
        public Node root;

        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = new Node("Program");
            root.Children.Add(Program());
            return root;
        }
        Node Program()
        {
            Node program = new Node("Program");
            program.Children.Add(Main_Function());
            MessageBox.Show("Success");

            return program;
        }
        Node Comment()
        {
            Node Comment = new Node("Comment");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.comment)
                {
                    Comment.Children.Add(match(Token_Class.comment));
                    return Comment;
                }
            }
            return null;
        }

        //done
        Node Function_call()
        {
            Node Function = new Node("Function_call");

            Function.Children.Add(match(Token_Class.Idenifier));
            Function.Children.Add(match(Token_Class.LParanthesis));
            Function.Children.Add(Arguments());
            Function.Children.Add(match(Token_Class.RParanthesis));
            return Function;
        }
        Node Arguments()
        {
            Node Arguments = new Node("Arguments");
            if (InputPointer < TokenStream.Count)
            {

                if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
                {
                    Arguments.Children.Add(match(Token_Class.Idenifier));
                    Arguments.Children.Add(many_Arguments());
                    return Arguments;
                }
                else if(TokenStream[InputPointer].token_type == Token_Class.Idenifier|| TokenStream[InputPointer].token_type == Token_Class.Number || TokenStream[InputPointer].token_type == Token_Class.LParanthesis || TokenStream[InputPointer].token_type == Token_Class.String)
                {
                    Arguments.Children.Add(Expression());
                    Arguments.Children.Add(many_Arguments());
                    return Arguments;
                }
                return null;
            }

            return null;

        }
        Node many_Arguments()
        {
            Node many_Arguments = new Node("many_Arguments");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Comma)
                {
                    many_Arguments.Children.Add(match(Token_Class.Comma));
                    if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
                    {
                        many_Arguments.Children.Add(match(Token_Class.Idenifier));
                    }
                    else if (TokenStream[InputPointer].token_type == Token_Class.Idenifier || TokenStream[InputPointer].token_type == Token_Class.Number || TokenStream[InputPointer].token_type == Token_Class.LParanthesis || TokenStream[InputPointer].token_type == Token_Class.String)
                    {
                        many_Arguments.Children.Add(Expression());
                    }
                    many_Arguments.Children.Add(this.many_Arguments());
                    return many_Arguments;
                }
            }
            return null;
        }
        Node Arithmatic_Operator()
        {
            Node Arithmatic_Operator = new Node("Arithmatic_Operator");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.PlusOp)
                {
                    Arithmatic_Operator.Children.Add(match(Token_Class.PlusOp));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.MinusOp)
                {
                    Arithmatic_Operator.Children.Add(match(Token_Class.MinusOp));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.MultiplyOp)
                {
                    Arithmatic_Operator.Children.Add(match(Token_Class.MultiplyOp));
                }
                else
                {
                    Arithmatic_Operator.Children.Add(match(Token_Class.DivideOp));
                }
            }
            return Arithmatic_Operator;

        }
        // done
        Node Term()
        {

            Node Term = new Node("Term");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Number)
                {
                    Term.Children.Add(match(Token_Class.Number));
                    return Term;
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.LParanthesis)
                {
                    Term.Children.Add(match(Token_Class.LParanthesis));
                    Term.Children.Add(EXP());
                    Term.Children.Add(match(Token_Class.RParanthesis));
                    return Term;
                }

                else
                {
                    if (InputPointer + 1 < TokenStream.Count)
                    {
                        if (TokenStream[InputPointer + 1].token_type == Token_Class.LParanthesis)
                        {
                            Term.Children.Add(Function_call());
                            return Term;
                        }
                    }

                    Term.Children.Add(match(Token_Class.Idenifier));
                    return Term;
                }


            }
            return null;
        }


        //done
        Node Expression()
        {
            Node Expression = new Node("Expression");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.String)
                {
                    Expression.Children.Add(match(Token_Class.String));
                    return Expression;
                }
                else
                {
                    Expression.Children.Add(Equation());
                    return Expression;
                }
                
            }
            return Expression;
        }

        Node ADD_OP()
        {
            Node ADD_OP = new Node("ADD_OP");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.PlusOp)
                {
                    ADD_OP.Children.Add(match(Token_Class.PlusOp));
                }
                else
                {
                    ADD_OP.Children.Add(match(Token_Class.MinusOp));
                }

            }
            return ADD_OP;

        }
        Node mult_OP()
        {
            Node mult_OP = new Node("mult_OP");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.MultiplyOp)
                {
                    mult_OP.Children.Add(match(Token_Class.MultiplyOp));
                }
                else
                {
                    mult_OP.Children.Add(match(Token_Class.DivideOp));
                }

            }
            return mult_OP;

        }
        Node EXP()
        {
            Node EXP = new Node("EXP");
            EXP.Children.Add(Term());
            EXP.Children.Add(EXP_dash());
            return EXP;
        }
        Node EXP_dash()
        {
            Node EXP_dash = new Node("EXP_dash");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.PlusOp || TokenStream[InputPointer].token_type == Token_Class.MinusOp)
                {
                    EXP_dash.Children.Add(ADD_OP());
                    EXP_dash.Children.Add(EXP());
                }
            }
            return EXP_dash;
        }
        Node Equation_dash()
        {
            Node Equation_dash = new Node("Equation_dash");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.MultiplyOp || TokenStream[InputPointer].token_type == Token_Class.DivideOp)
                {
                    Equation_dash.Children.Add(mult_OP());
                    Equation_dash.Children.Add(Equation());
                }
            }
            return Equation_dash;
        }


        Node Equation()
        {
            Node Equation = new Node(" Equation");

            Equation.Children.Add(EXP());
            Equation.Children.Add(Equation_dash());

            return Equation;

        }


        //done
        Node Assignment_Statement()
        {
            Node Assignment_Statement = new Node("Assignment_Statement");

            Assignment_Statement.Children.Add(match(Token_Class.Idenifier));
            Assignment_Statement.Children.Add(match(Token_Class.Assign));
            Assignment_Statement.Children.Add(Expression());
           // Assignment_Statement.Children.Add(match(Token_Class.Semicolon));



            return Assignment_Statement;
        }

        Node DataType()
        {
            Node DataType = new Node("DataType");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Integer)
                {
                    DataType.Children.Add(match(Token_Class.Integer));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.FLOAT)
                {
                    DataType.Children.Add(match(Token_Class.FLOAT));
                }
                else
                {
                    DataType.Children.Add(match(Token_Class.String));
                }
            }
            return DataType;
        }

        //done
        Node Declaration_Statement()
        {
            Node Declaration_Statement = new Node("Declaration_Statement");
            Declaration_Statement.Children.Add(DataType());
            Declaration_Statement.Children.Add(one_Identi_assi());
            Declaration_Statement.Children.Add(match(Token_Class.Semicolon));
            return Declaration_Statement;
        }
        Node one_Identi_assi()
        {
            Node one = new Node("one_Identi_assi");
            one.Children.Add(Identi_assi());
            one.Children.Add(more_Identi_assi());
            return one;
        }
        Node more_Identi_assi()
        {
            Node more = new Node("more_Identi_assi");
            if (TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                more.Children.Add(match(Token_Class.Comma));
                more.Children.Add(Identi_assi());
                more.Children.Add(more_Identi_assi());
            }
            return more;
        }
        Node Identi_assi()
        {
            Node New_Identifier = new Node("New_Identifier");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
                {
                    if (InputPointer + 1 < TokenStream.Count)
                    {
                        if (TokenStream[InputPointer + 1].token_type == Token_Class.Assign)
                        {
                            New_Identifier.Children.Add(Assignment_Statement());
                            New_Identifier.Children.Add(match(Token_Class.Semicolon));
                            return New_Identifier;
                        }
                    }
                    New_Identifier.Children.Add(match(Token_Class.Idenifier));
                    return New_Identifier;
                }

                return null;
            }
            return null;
        }

        Node write_statement()
        {
            Node write_statement = new Node("write_statement");
            write_statement.Children.Add(match(Token_Class.Write));
            if (TokenStream[InputPointer].token_type == Token_Class.Idenifier || TokenStream[InputPointer].token_type == Token_Class.Number || TokenStream[InputPointer].token_type == Token_Class.LParanthesis || TokenStream[InputPointer].token_type == Token_Class.String)
            {
                write_statement.Children.Add(Expression());
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.ENDL)
            {
                write_statement.Children.Add(match(Token_Class.ENDL));
            }
            write_statement.Children.Add(match(Token_Class.Semicolon));
            return write_statement;
        }
        Node read_statement()
        {
            Node read_statement = new Node("read_statement");
            read_statement.Children.Add(match(Token_Class.Read));
            read_statement.Children.Add(match(Token_Class.Idenifier));
            read_statement.Children.Add(match(Token_Class.Semicolon));
            return read_statement;
        }
        Node return_statement()
        {
            Node return_statement = new Node("return_statement");
            return_statement.Children.Add(match(Token_Class.RETURN));
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
                {
                    return_statement.Children.Add(match(Token_Class.Idenifier));
                }

                else
                {
                    return_statement.Children.Add(match(Token_Class.Number));
                }
            }
            return_statement.Children.Add(match(Token_Class.Semicolon));
            return return_statement;
        }

        //done
        Node Condition_statement()
        {
            Node Condition_statement = new Node("Condition_statement");
            Condition_statement.Children.Add(Condition());
            Condition_statement.Children.Add(Conditions());
            return Condition_statement;
        }
        Node Conditions()
        {
            Node Conditions = new Node("Conditions");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.and || TokenStream[InputPointer].token_type == Token_Class.or)
                {
                    Conditions.Children.Add(match(TokenStream[InputPointer].token_type));
                    Conditions.Children.Add(Condition());
                    Conditions.Children.Add(this.Conditions());
                    return Conditions;
                }
                return null;
            }
            return null;
        }
        Node Condition()
        {
            Node Condition = new Node("Condition");
            Condition.Children.Add(match(Token_Class.Idenifier));
            Condition.Children.Add(Condition_Operator());
            Condition.Children.Add(Term());
            return Condition;
        }
        /*Node Condition()
        {
            Node Condition = new Node("Condition");
            Condition.Children.Add(match(Token_Class.Idenifier));
            if (Condition_Operator() != null && Term() != null)
            {
                Condition.Children.Add(Condition_Operator());
                if (Term() != null)
                {
                    Condition.Children.Add(Term());
                }
            }
            
            return Condition;
        }*/
        Node Condition_Operator()
        {
            Node Condition_Operator = new Node("Condition_Operator");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.LessThanOp)
                {
                    Condition_Operator.Children.Add(match(Token_Class.LessThanOp));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.GreaterThanOp)
                {
                    Condition_Operator.Children.Add(match(Token_Class.GreaterThanOp));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.EqualOp)
                {
                    Condition_Operator.Children.Add(match(Token_Class.EqualOp));
                }
                else
                {
                    Condition_Operator.Children.Add(match(Token_Class.NotEqualOp));
                }
                return Condition_Operator;

            }
            return null;
        }

        ////done 
        Node Body()
        {
            Node Bodys = new Node("Body");
            if (InputPointer < TokenStream.Count)
            {
                Node TMPs = new Node("TMPs");
                if (TokenStream[InputPointer].token_type == Token_Class.Integer || TokenStream[InputPointer].token_type == Token_Class.FLOAT || TokenStream[InputPointer].token_type == Token_Class.String)
                {
                    Bodys.Children.Add(Declaration_Statement());
                    Bodys.Children.Add(Body());
                    return Bodys;
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Read)
                {
                    Bodys.Children.Add(read_statement());
                    Bodys.Children.Add(Body());
                    return Bodys;
                }

                else if (TokenStream[InputPointer].token_type == Token_Class.Write)
                {
                    Bodys.Children.Add(write_statement());
                    Bodys.Children.Add(Body());
                    return Bodys;
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Repeat)
                {
                    Bodys.Children.Add(Repeat());
                    Bodys.Children.Add(Body());
                    return Bodys;
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.RETURN)
                {
                    Bodys.Children.Add(return_statement());
                    Bodys.Children.Add(Body());
                    return Bodys;
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
                {
                    Bodys.Children.Add(Assignment_Statement());
                    Bodys.Children.Add(match(Token_Class.Semicolon));
                    Bodys.Children.Add(Body());
                    return Bodys;
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.comment)
                {
                    Bodys.Children.Add(match(Token_Class.comment));
                    return Bodys;
                }
                return null;
            }
            return null;
        }
        Node if_statement()
        {
            Node if_statement = new Node("if_statement");
            if_statement.Children.Add(match(Token_Class.IF));
            if_statement.Children.Add(Condition_statement());
            if_statement.Children.Add(match(Token_Class.Then));
            if_statement.Children.Add(Body());
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.ELSEIF)
                {
                    if_statement.Children.Add(elseif_statement());
                    return if_statement;
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.ELSE)
                {
                    if_statement.Children.Add(else_statement());
                    return if_statement;
                }

            }
            if_statement.Children.Add(match(Token_Class.END));

            return if_statement;
        }
        Node elseif_statement()
        {
            Node elseif_statement = new Node("elseif_statement");
            elseif_statement.Children.Add(match(Token_Class.ELSEIF));
            elseif_statement.Children.Add(Condition_statement());
            elseif_statement.Children.Add(match(Token_Class.Then));
            elseif_statement.Children.Add(Body());
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.ELSEIF)
                {
                    elseif_statement.Children.Add(this.elseif_statement());
                    return elseif_statement;
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.ELSE)
                {
                    elseif_statement.Children.Add(else_statement());
                    return elseif_statement;
                }
            }
            elseif_statement.Children.Add(match(Token_Class.END));
            return elseif_statement;
        }
        Node else_statement()
        {
            Node else_statement = new Node("else_statement");
            else_statement.Children.Add(match(Token_Class.ELSE));
            else_statement.Children.Add(Body());
            else_statement.Children.Add(match(Token_Class.END));
            return else_statement;
        }

        ////done 
        Node Repeat()
        {
            Node Repeat = new Node("Repeat");
            Repeat.Children.Add(match(Token_Class.Repeat));
            Repeat.Children.Add(REP_Body_dash());
            //Repeat.Children.Add(match(Token_Class.Semicolon));
            Repeat.Children.Add(match(Token_Class.UNTIL));
            Repeat.Children.Add(Condition_statement());
            return Repeat;
        }
        Node REP_Body_dash()
        {
            Node dash = new Node("Body");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Integer || TokenStream[InputPointer].token_type == Token_Class.FLOAT || TokenStream[InputPointer].token_type == Token_Class.String)
                {
                    dash.Children.Add(Declaration_Statement());
                    dash.Children.Add(REP_Body_dash());
                    return dash;
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Read)
                {
                    dash.Children.Add(read_statement());
                    dash.Children.Add(REP_Body_dash());
                    return dash;
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Write)
                {
                    dash.Children.Add(write_statement());
                    dash.Children.Add(REP_Body_dash());
                    return dash;
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.RETURN)
                {
                    dash.Children.Add(return_statement());
                    dash.Children.Add(REP_Body_dash());
                    return dash;
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
                {
                    dash.Children.Add(Assignment_Statement());
                    dash.Children.Add(match(Token_Class.Semicolon));
                    dash.Children.Add(REP_Body_dash());
                    return dash;
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.IF)
                {
                    dash.Children.Add(if_statement());
                    dash.Children.Add(REP_Body_dash());
                    return dash;
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.comment)
                {
                    dash.Children.Add(match(Token_Class.comment));
                    return dash;
                }
                return null;
            }

            return null;
        }
        //done 
        Node Function_declaration()
        {
            Node Function_declaration = new Node("Function_declaration");

            Function_declaration.Children.Add(DataType());
            Function_declaration.Children.Add(Function_name());
            Function_declaration.Children.Add(match(Token_Class.LParanthesis));
            Function_declaration.Children.Add(Parameters());
            Function_declaration.Children.Add(match(Token_Class.RParanthesis));

            return Function_declaration;
        }
        Node Function_name()
        {
            Node Function_name = new Node("Function_name");
            Function_name.Children.Add(match(Token_Class.Idenifier));
            return Function_name;
        }
        Node Parameters()
        {
            Node Parameters = new Node("Parameters");
            Parameters.Children.Add(Parameter());
            Parameters.Children.Add(this.P());
            return Parameters;
        }
        Node P()
        {
            Node Pp = new Node("Parameter");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Comma)
                {
                    Pp.Children.Add(match(Token_Class.Comma));
                    Pp.Children.Add(Parameter());
                    Pp.Children.Add(P());
                }
            }
            return Pp;
        }
        Node Parameter()
        {
            Node Parameter = new Node("Parameter");
            Parameter.Children.Add(DataType());
            Parameter.Children.Add(match(Token_Class.Idenifier));
            return Parameter;
        }

        //done  
        Node Function_Body()
        {
            Node Funct_Body = new Node("Function_Body");
            Funct_Body.Children.Add(match(Token_Class.LeftBrances));
            Funct_Body.Children.Add(Comment());
            Funct_Body.Children.Add(f_state());        
            Funct_Body.Children.Add(return_statement());
            Funct_Body.Children.Add(Comment());
            Funct_Body.Children.Add(match(Token_Class.RightBrances));
            return Funct_Body;
        }
        Node f_state()
        {
            Node f_TMP = new Node("f_state");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Integer || TokenStream[InputPointer].token_type == Token_Class.FLOAT || TokenStream[InputPointer].token_type == Token_Class.String)
                {
                    f_TMP.Children.Add(Declaration_Statement());
                    f_TMP.Children.Add(f_state());
                    return f_TMP;
                }
               else if (TokenStream[InputPointer].token_type == Token_Class.comment)
                {
                    f_TMP.Children.Add(Comment());

                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Read)
                {
                    f_TMP.Children.Add(read_statement());
                    f_TMP.Children.Add(f_state());
                    return f_TMP;
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Write)
                {
                    f_TMP.Children.Add(write_statement());
                    f_TMP.Children.Add(f_state());
                    return f_TMP;
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
                {
                    f_TMP.Children.Add(Assignment_Statement());
                    f_TMP.Children.Add(match(Token_Class.Semicolon));
                    f_TMP.Children.Add(f_state());
                    return f_TMP;
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.IF)
                {
                    f_TMP.Children.Add(if_statement());
                    f_TMP.Children.Add(f_state());
                    return f_TMP;
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Repeat)
                {
                    f_TMP.Children.Add(Repeat());
                    f_TMP.Children.Add(f_state());
                    return f_TMP;
                }
                return null;
            }
            return null;
        }

        //done
        Node Function_Statement()
        {
            Node Function_Statement = new Node("Function_Statement");
            Function_Statement.Children.Add(Function_declaration());
            Function_Statement.Children.Add(Function_Body());
            return Function_Statement;
        }
        Node Main_Function()
        {
            Node Main_Funct = new Node("Main_Function");
            Main_Funct.Children.Add(Comment());
            Main_Funct.Children.Add(DataType());
            Main_Funct.Children.Add(Comment());
            Main_Funct.Children.Add(match(Token_Class.MAIN));
            Main_Funct.Children.Add(Comment());
            Main_Funct.Children.Add(match(Token_Class.LParanthesis));
            Main_Funct.Children.Add(Comment());
            Main_Funct.Children.Add(match(Token_Class.RParanthesis));
            Main_Funct.Children.Add(Comment());
            Main_Funct.Children.Add(Function_Body());
            return Main_Funct;
        }

        Node Functions()
        {
            Node Functions = new Node("Functions");
            Functions.Children.Add(Function_Statement());
             if (InputPointer+1 < TokenStream.Count)
             {
                 if (TokenStream[InputPointer + 1].token_type == Token_Class.Idenifier)
                 {
                    Functions.Children.Add(this.Functions());
                }
             }
            

            return Functions;
        }

        public Node match(Token_Class ExpectedToken)
        {

            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;

                }

                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + "\r\n");
                InputPointer++;
                return null;
            }
        }

        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}

