import * as path from 'path';
import { Configuration } from 'webpack';
const Dotenv = require('dotenv-webpack');

const config: Configuration = {
  entry: './src/index.ts', // Adjust the entry point as needed
  output: {
    path: path.resolve(__dirname, 'dist'),
    filename: 'bundle.js',
    //publicPath: '',//S3 location. Need to upload to s3 in ci/cd - aws s3 sync dist/ s3://your-s3-bucket-name/ --acl public-read
  },
  resolve: {
    extensions: ['.ts', '.js'],
  },
  module: {
    rules: [
      {
        test: /\.ts$/,
        use: 'ts-loader',
        exclude: /node_modules/,
      },
    ],
  },
  plugins: [
    new Dotenv({
      path: path.resolve(__dirname, process.env.NODE_ENV === 'development' ? '.env.development' : '.env'),
    }),
  ],
};

export default config;
